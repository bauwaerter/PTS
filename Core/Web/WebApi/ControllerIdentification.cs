using System;
using System.Collections.Generic;
using System.Globalization;

namespace Core.Web.WebApi
{
    /// <summary>
    /// Represents a controller name with an associated version
    /// </summary>
    public struct ControllerIdentification : IEquatable<ControllerIdentification>
    {
        private static readonly Lazy<IEqualityComparer<ControllerIdentification>> ComparerInstance = new Lazy<IEqualityComparer<ControllerIdentification>>(() => new ControllerNameComparer());

        /// <summary>
        /// Gets an comparer for comparing <see cref="ControllerIdentification"/> instances
        /// </summary>
        public static IEqualityComparer<ControllerIdentification> Comparer
        {
            get { return ComparerInstance.Value; }
        }

        /// <summary>
        /// Gets or sets the name of the controller
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the associated version
        /// </summary>
        public int? Version { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerIdentification"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        public ControllerIdentification(string name, int? version)
            : this()
        {
            this.Name = name;
            this.Version = version;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(ControllerIdentification other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(other.Name, this.Name) &&
                   other.Version == this.Version;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ControllerIdentification)
            {
                ControllerIdentification cn = (ControllerIdentification)obj;
                return this.Equals(cn);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().ToUpperInvariant().GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.Version == null)
            {
                return this.Name;
            }

            return VersionedControllerSelector.VersionPrefix + this.Version.Value.ToString(CultureInfo.InvariantCulture) + "." + this.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        private class ControllerNameComparer : IEqualityComparer<ControllerIdentification>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type T to compare.</param>
            /// <param name="y">The second object of type T to compare.</param>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            public bool Equals(ControllerIdentification x, ControllerIdentification y)
            {
                return x.Equals(y);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The obj.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(ControllerIdentification obj)
            {
                return obj.GetHashCode();
            }
        }

    } // class
} // namespace