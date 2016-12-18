﻿//-----------------------------------------------------
//Apache2, 2014-2016,   WinterDev
//some logics from FreeType Lib (FTL, BSD-3 clause)
//-----------------------------------------------------

using System;
namespace NOpenType
{
    public abstract class GlyphPathBuilderBase
    {
        readonly Typeface _typeface;
        public GlyphPathBuilderBase(Typeface typeface)
        {
            _typeface = typeface;
        }
        struct FtPoint
        {
            readonly short _x;
            readonly short _y;
            public FtPoint(short x, short y)
            {
                _x = x;
                _y = y;
            }
            public short X { get { return _x; } }
            public short Y { get { return _y; } }

            public override string ToString()
            {
                return "(" + _x + "," + _y + ")";
            }
        }
        protected abstract void OnBeginRead(int countourCount);
        protected abstract void OnEndRead();
        protected abstract void OnCloseFigure();
        protected abstract void OnCurve3(short p2x, short p2y, short x, short y);
        protected abstract void OnCurve4(short p2x, short p2y, short p3x, short p3y, short x, short y);
        protected abstract void OnMoveTo(short x, short y);
        protected abstract void OnLineTo(short x, short y);

        void RenderGlyph(ushort[] contours, short[] xs, short[] ys, bool[] onCurves)
        {

            //outline version
            //-----------------------------
            int npoints = xs.Length;
            int startContour = 0;
            int cpoint_index = 0;
            int todoContourCount = contours.Length;
            //----------------------------------- 
            OnBeginRead(todoContourCount);
            //-----------------------------------
            short lastMoveX = 0;
            short lastMoveY = 0;


            int controlPointCount = 0;
            while (todoContourCount > 0)
            {
                int nextContour = contours[startContour] + 1;
                bool isFirstPoint = true;
                FtPoint secondControlPoint = new FtPoint();
                FtPoint thirdControlPoint = new FtPoint();


                bool justFromCurveMode = false;
                for (; cpoint_index < nextContour; ++cpoint_index)
                {

                    short vpoint_x = xs[cpoint_index];
                    short vpoint_y = ys[cpoint_index];
                    //int vtag = (int)flags[cpoint_index] & 0x1;
                    //bool has_dropout = (((vtag >> 2) & 0x1) != 0);
                    //int dropoutMode = vtag >> 3;
                    if (onCurves[cpoint_index])
                    {
                        //on curve
                        if (justFromCurveMode)
                        {
                            switch (controlPointCount)
                            {
                                case 1:
                                    {
                                        OnCurve3(secondControlPoint.X, secondControlPoint.Y,
                                            vpoint_x, vpoint_y);
                                    }
                                    break;
                                case 2:
                                    {
                                        OnCurve4(secondControlPoint.X, secondControlPoint.Y,
                                             thirdControlPoint.X, thirdControlPoint.Y,
                                             vpoint_x, vpoint_y);
                                    }
                                    break;
                                default:
                                    {
                                        throw new NotSupportedException();
                                    }
                            }
                            controlPointCount = 0;
                            justFromCurveMode = false;
                        }
                        else
                        {
                            if (isFirstPoint)
                            {
                                isFirstPoint = false;
                                OnMoveTo(lastMoveX = (vpoint_x), lastMoveY = (vpoint_y));
                            }
                            else
                            {
                                OnLineTo(vpoint_x, vpoint_y);
                            }

                            //if (has_dropout)
                            //{
                            //    //printf("[%d] on,dropoutMode=%d: %d,y:%d \n", mm, dropoutMode, vpoint.x, vpoint.y);
                            //}
                            //else
                            //{
                            //    //printf("[%d] on,x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                            //}
                        }
                    }
                    else
                    {
                        switch (controlPointCount)
                        {
                            case 0:
                                {
                                    secondControlPoint = new FtPoint(vpoint_x, vpoint_y);
                                }
                                break;
                            case 1:
                                {

                                    //we already have prev second control point
                                    //so auto calculate line to 
                                    //between 2 point
                                    FtPoint mid = GetMidPoint(secondControlPoint, vpoint_x, vpoint_y);
                                    //----------
                                    //generate curve3
                                    OnCurve3(secondControlPoint.X, secondControlPoint.Y,
                                        mid.X, mid.Y);
                                    //------------------------
                                    controlPointCount--;
                                    //------------------------
                                    //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                    secondControlPoint = new FtPoint(vpoint_x, vpoint_y);

                                }
                                break;
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                                break;
                        }

                        controlPointCount++;
                        justFromCurveMode = true;
                    }
                }
                //--------
                //close figure
                //if in curve mode
                if (justFromCurveMode)
                {
                    switch (controlPointCount)
                    {
                        case 0: break;
                        case 1:
                            {
                                OnCurve3(secondControlPoint.X, secondControlPoint.Y,
                                    lastMoveX, lastMoveY);
                            }
                            break;
                        case 2:
                            {
                                OnCurve4(secondControlPoint.X, secondControlPoint.Y,
                                    thirdControlPoint.X, thirdControlPoint.Y,
                                    lastMoveX, lastMoveY);
                            }
                            break;
                        default:
                            { throw new NotSupportedException(); }
                    }
                    justFromCurveMode = false;
                    controlPointCount = 0;
                }
                OnCloseFigure();
                //--------                   
                startContour++;
                todoContourCount--;
            }
            OnEndRead();
        }

        static FtPoint GetMidPoint(FtPoint v1, short v2x, short v2y)
        {
            return new FtPoint(
                (short)((v1.X + v2x) >> 1),
                (short)((v1.Y + v2y) >> 1));
        }

        void RenderGlyph(Glyph glyph)
        {
            RenderGlyph(glyph.EndPoints, glyph.Xs, glyph.Ys, glyph.OnCurves);
        }

        public void Build(char c, float sizeInPoints)
        {
            BuildFromGlyphIndex((ushort)_typeface.LookupIndex(c), sizeInPoints);
        }
        public void BuildFromGlyphIndex(ushort glyphIndex, float sizeInPoints)
        {
            this.SizeInPoints = sizeInPoints;
            RenderGlyph(_typeface.GetGlyphByIndex(glyphIndex));
        }
        public float SizeInPoints
        {
            get;
            set;
        }

        protected Typeface TypeFace
        {
            get { return _typeface; }
        }
        protected ushort TypeFaceUnitPerEm
        {
            get { return _typeface.UnitsPerEm; }
        }

    }
}