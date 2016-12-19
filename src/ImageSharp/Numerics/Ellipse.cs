// <copyright file="Ellipse.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
    using System;
    using System.ComponentModel;
    using System.Numerics;

    /// <summary>
    /// Represents an ellipse.
    /// </summary>
    public struct Ellipse : IEquatable<Ellipse>
    {
        /// <summary>
        /// Represents a <see cref="Ellipse"/> that has X and Y values set to zero.
        /// </summary>
        public static readonly Ellipse Empty = default(Ellipse);

        /// <summary>
        /// The center point.
        /// </summary>
        private Vector2 center;
        private Vector2 radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> struct.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="radiusX">The x-radius.</param>
        /// <param name="radiusY">The y-radius.</param>
        public Ellipse(Vector2 center, float radiusX, float radiusY)
        {
            this.center = center;
            radius = new Vector2(radiusX, radiusY);
        }

        /// <summary>
        /// Gets the x-radius of this <see cref="Ellipse"/>.
        /// </summary>
        public float RadiusX => radius.X;

        /// <summary>
        /// Gets the y-radius of this <see cref="Ellipse"/>.
        /// </summary>
        public float RadiusY => radius.Y;

        /// <summary>
        /// Gets the x of center of this <see cref="Ellipse"/>.
        /// </summary>
        public float X => center.X;

        /// <summary>
        /// Gets the y of center of this <see cref="Ellipse"/>.
        /// </summary>
        public float Y => center.Y;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Ellipse"/> is empty.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsEmpty => this.Equals(Empty);

        /// <summary>
        /// Compares two <see cref="Ellipse"/> objects for equality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Ellipse"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Ellipse"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Ellipse left, Ellipse right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Ellipse"/> objects for inequality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Ellipse"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Ellipse"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Ellipse left, Ellipse right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns the center point of the given <see cref="Ellipse"/>
        /// </summary>
        /// <param name="ellipse">The ellipse</param>
        /// <returns><see cref="Vector2"/></returns>
        public static Vector2 Center(Ellipse ellipse)
        {
            return ellipse.center;
        }

        /// <summary>
        /// Determines if the specfied point is contained within the rectangular region defined by
        /// this <see cref="Ellipse"/>.
        /// </summary>
        /// <param name="x">The x-coordinate of the given point.</param>
        /// <param name="y">The y-coordinate of the given point.</param>
        /// <returns>The <see cref="bool"/></returns>
        public bool Contains(int x, int y)
        {
            if (this.RadiusX <= 0 || this.RadiusY <= 0)
            {
                return false;
            }

            Vector2 point = new Vector2(x, y);
            Vector2 normalized = point - this.center;
            var normalizedSqr = normalized * normalized;
            var radiusSqr = radius * radius;
            float nX = normalized.X;
            float nY = normalized.Y;

            var ratio = normalizedSqr / radiusSqr;

            return (ratio.X + ratio.Y) <= 1.0;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.GetHashCode(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.IsEmpty)
            {
                return "Ellipse [ Empty ]";
            }

            return
                $"Ellipse [ RadiusX={this.RadiusX}, RadiusY={this.RadiusX}, Centre={this.center.X},{this.center.Y} ]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Ellipse)
            {
                return this.Equals((Ellipse)obj);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Equals(Ellipse other)
        {
            return this.center.Equals(other.center)
                && this.RadiusX.Equals(other.RadiusX)
                && this.RadiusY.Equals(other.RadiusY);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <param name="ellipse">
        /// The instance of <see cref="Point"/> to return the hash code for.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        private int GetHashCode(Ellipse ellipse)
        {
            unchecked
            {
                int hashCode = ellipse.center.GetHashCode();
                hashCode = (hashCode * 397) ^ ellipse.RadiusX.GetHashCode();
                hashCode = (hashCode * 397) ^ ellipse.RadiusY.GetHashCode();
                return hashCode;
            }
        }
    }
}