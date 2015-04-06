namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    internal class StorableCookie
    {
        private Cookie innerCookie;

        public StorableCookie()
        {
        }

        public StorableCookie(Cookie cookie)
        {
            this.innerCookie = cookie;
        }

        public Cookie OriginCookie
        {
            get
            {
                if (innerCookie == null)
                {
                    innerCookie = new Cookie();
                }

                return innerCookie;
            }
        }

        [DataMember]
        public string Comment
        {
            get
            {
                return this.OriginCookie.Comment;
            }
            set
            {
                this.OriginCookie.Comment = value;
            }
        }

        /// <summary>
        /// Gets or sets a URI comment that the server can provide with a System.Net.Cookie.
        /// </summary>
        [DataMember]
        public Uri CommentUri
        {
            get
            {
                return this.OriginCookie.CommentUri;
            }
            set
            {
                this.OriginCookie.CommentUri = value;
            }
        }

        /// <summary>
        /// Gets or sets the discard flag set by the server.
        /// </summary>
        [DataMember]
        public bool Discard
        {
            get
            {
                return this.OriginCookie.Discard;
            }
            set
            {
                this.OriginCookie.Discard = value;
            }
        }

        /// <summary>
        /// Gets or sets the URI for which the System.Net.Cookie is valid.
        /// </summary>
        [DataMember]
        public string Domain
        {
            get
            {
                return this.OriginCookie.Domain;
            }
            set
            {
                if (value.StartsWith("."))
                {
                    this.OriginCookie.Domain = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current state of the System.Net.Cookie.
        /// </summary>
        [DataMember]
        public bool Expired
        {
            get
            {
                return this.OriginCookie.Expired;
            }
            set
            {
                this.OriginCookie.Expired = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration date and time for the System.Net.Cookie as a <see cref="System.DateTime"/>.
        /// </summary>
        public DateTime Expires
        {
            get
            {
                return this.OriginCookie.Expires;
            }
            set
            {
                this.OriginCookie.Expires = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration date and time for the System.Net.Cookie in ticks form.
        /// </summary>
        [DataMember]
        public long ExpiresTicks
        {
            get
            {
                var ticks = this.Expires.ToUniversalTime().Ticks;
                return ticks;
            }

            set
            {
                var newDateTime = new DateTime(value, DateTimeKind.Utc);
                this.Expires = newDateTime;
            }
        }

        /// <summary>
        /// Determines whether a page script or other active content can access this cookie.
        /// </summary>
        [DataMember]
        public bool HttpOnly
        {
            get
            {
                return this.OriginCookie.HttpOnly;
            }
            set
            {
                this.OriginCookie.HttpOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets the name for the System.Net.Cookie.
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return this.OriginCookie.Name;
            }
            set
            {
                this.OriginCookie.Name = value;
            }
        }

        /// <summary>
        ///  Gets or sets the URIs to which the System.Net.Cookie applies.
        /// </summary>
        [DataMember]
        public string Path
        {
            get
            {
                return this.OriginCookie.Path;
            }
            set
            {
                this.OriginCookie.Path = value;
            }
        }

        /// <summary>
        /// Gets or sets a list of TCP ports that the System.Net.Cookie applies to.
        /// </summary>
        [DataMember]
        public string Port
        {
            get
            {
                return this.OriginCookie.Port;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    this.OriginCookie.Port = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the security level of a System.Net.Cookie.
        /// </summary>
        [DataMember]
        public bool Secure
        {
            get
            {
                return this.OriginCookie.Secure;
            }
            set
            {
                this.OriginCookie.Secure = value;
            }
        }

        /// <summary>
        /// Gets the time when the cookie was issued as a System.DateTime.
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                return this.OriginCookie.TimeStamp;
            }
        }

        /// <summary>
        /// Gets or sets the System.Net.Cookie.Value for the System.Net.Cookie.
        /// </summary>
        [DataMember]
        public string Value
        {
            get
            {
                return this.OriginCookie.Value;
            }
            set
            {
                this.OriginCookie.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of HTTP state maintenance to which the cookie conforms.
        /// </summary>
        [DataMember]
        public int Version
        {
            get
            {
                return this.OriginCookie.Version;
            }
            set
            {
                this.OriginCookie.Version = value;
            }
        }
    }
}
