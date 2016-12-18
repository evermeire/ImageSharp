// <copyright file="FillShapeProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using Drawing;
    using ImageSharp.Processors;
    using Shapes;
    using System.Buffers;

    /// <summary>
    /// Usinf a brsuh and a shape fills shape with contents of brush the
    /// </summary>
    /// <typeparam name="TColor">The type of the color.</typeparam>
    /// <typeparam name="TPacked">The type of the packed.</typeparam>
    /// <seealso cref="ImageSharp.Processors.ImageFilteringProcessor{TColor, TPacked}" />
    public class FillShapeProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct, IEquatable<TPacked>
    {
        private const float Epsilon = 0.001f;

        private const float AntialiasFactor = 1f;
        private const int DrawPadding = 1;
        private readonly IBrush<TColor, TPacked> fillColor;
        private readonly IShape poly;
        private readonly GraphicsOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FillShapeProcessor{TColor, TPacked}"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="options">The graphics options.</param>
        public FillShapeProcessor(IBrush<TColor, TPacked> brush, IShape shape, GraphicsOptions options)
        {
            this.poly = shape;
            this.fillColor = brush;
            this.options = options;
        }

        static void QuickSort(Vector2[] data, int left, int right)
        {
            int i = left - 1,
                j = right;

            while (true)
            {
                float d = data[left].X;
                do i++; while (data[i].X < d);
                do j--; while (data[j].X > d);

                if (i < j)
                {
                    Vector2 tmp = data[i];
                    data[i] = data[j];
                    data[j] = tmp;
                }
                else
                {
                    if (left < j) QuickSort(data, left, j);
                    if (++j < right) QuickSort(data, j, right);
                    return;
                }
            }
        }
        /// <inheritdoc/>
        protected override void OnApply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle)
        {
            var rect = RectangleF.Ceiling(this.poly.Bounds); // rounds the points out away from the center

            int polyStartY = rect.Y - DrawPadding;
            int polyEndY = rect.Bottom + DrawPadding;
            int startX = rect.X - DrawPadding;
            int endX = rect.Right + DrawPadding;

            int minX = Math.Max(sourceRectangle.Left, startX);
            int maxX = Math.Min(sourceRectangle.Right, endX);
            int minY = Math.Max(sourceRectangle.Top, polyStartY);
            int maxY = Math.Min(sourceRectangle.Bottom, polyEndY);

            // Align start/end positions.
            minX = Math.Max(0, minX);
            maxX = Math.Min(source.Width, maxX);
            minY = Math.Max(0, minY);
            maxY = Math.Min(source.Height, maxY);

            // Reset offset if necessary.
            if (minX > 0)
            {
                startX = 0;
            }

            if (minY > 0)
            {
                polyStartY = 0;
            }
            ArrayPool<Vector2> arrayPool = ArrayPool<Vector2>.Shared;

            int maxIntersections = poly.MaxIntersections;

#if DEBUG
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            using (IBrushApplicator<TColor, TPacked> applicator = this.fillColor.CreateApplicator(rect))
            {

                // we need to repeat this vertically to set anitialiasing vertically
                // but we only have to get colors/fills for the external points nearest transitions in the X Pass ands only is anitialiasing is enabled
                Parallel.For(
                minY,
                maxY,
                this.ParallelOptions,
                y =>
                {
                    int offsetY = y - polyStartY;
                    if(offsetY == 55)
                    {
                        var t = 2;
                    }

                    var buffer = arrayPool.Rent(maxIntersections);
                    var left = new Vector2(startX, offsetY);
                    var right = new Vector2(endX, offsetY);
                    // foreach line we get all the points where this line crosses the polygon

                    var pointsFound = poly.FindIntersections(left, right, buffer, maxIntersections, 0);
                    if(pointsFound == 0)
                    {
                        arrayPool.Return(buffer);
                        //nothign on this line skip
                        return;
                    }

                    QuickSort(buffer, 0, pointsFound);

                    int currentIntersection = 0;
                    float nextPoint = buffer[0].X;
                    float lastPoint = left.X;
                    float targetPoint = nextPoint;
                    bool isInside = false;
                    // every odd point is the start of a line

                    Vector2 currentPoint = default(Vector2);
                    for (int x = minX; x < maxX; x++)
                    {
                        int offsetX = x - startX;
                        currentPoint.X = offsetX;
                        currentPoint.Y = offsetY;
                        if (!isInside)
                        {
                            if(offsetX < (nextPoint - DrawPadding) && offsetX > (lastPoint + DrawPadding))
                            {
                                if(nextPoint == right.X)
                                {
                                    // we are in the ends run skip it
                                    x = maxX;
                                    continue;
                                }
                                //lest just jump forward
                                x = (int)Math.Floor(nextPoint) + startX - DrawPadding; //orer is
                            }
                        }
                        bool onCorner = false;
                        if (offsetX >= nextPoint) //there seems to be some issue with this switch.
                        {
                            currentIntersection++;
                            lastPoint = nextPoint;
                            if (currentIntersection == pointsFound)
                            {
                                nextPoint = right.X;
                            }
                            else
                            {
                                nextPoint = buffer[currentIntersection].X;
                                //double point from a corner flip the bit back and move on again
                                if (nextPoint == lastPoint)
                                {
                                    onCorner = true;
                                    isInside ^= true;
                                    currentIntersection++;
                                    if (currentIntersection == pointsFound)
                                    {

                                        nextPoint = right.X;
                                    }
                                    else
                                    {
                                        nextPoint = buffer[currentIntersection].X;
                                    }
                                }
                            }

                            isInside ^= true;
                        }

                        float opacity = 1;
                        if (!isInside && !onCorner)
                        {
                            if (this.options.Antialias)
                            {
                                float distance = float.MaxValue;
                                if (offsetX == lastPoint || offsetX == nextPoint)
                                {
                                    //we are to far away from the line
                                    distance = 0;
                                }
                                else if(nextPoint - AntialiasFactor < offsetX)
                                {
                                    //we are near the left of the line 
                                    distance = nextPoint - offsetX;
                                }else if (lastPoint + AntialiasFactor > offsetX)
                                {
                                    //we are near the right of the line 
                                    distance = offsetX - lastPoint;
                                }
                                else
                                {
                                    //we are to far away from the line
                                    continue;
                                }
                                opacity = 1 - (distance / AntialiasFactor);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (opacity > Epsilon)
                        {
                            Vector4 backgroundVector = sourcePixels[offsetX, offsetY].ToVector4();
                            Vector4 sourceVector = applicator.GetColor(currentPoint).ToVector4();

                            var finalColor = Vector4BlendTransforms.PremultipliedLerp(backgroundVector, sourceVector, opacity);
                            finalColor.W = backgroundVector.W;

                            TColor packed = default(TColor);
                            packed.PackFromVector4(finalColor);
                            sourcePixels[offsetX, offsetY] = packed;
                        }
                    }
                    
                    arrayPool.Return(buffer);
                });
            }
        }
    }
}