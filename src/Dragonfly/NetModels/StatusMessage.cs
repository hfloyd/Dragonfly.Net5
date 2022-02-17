namespace Dragonfly.NetModels
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    /// <summary>
    /// Object used for collecting and reporting information about code operations.
    /// </summary>
    public class StatusMessage
    {
        private Type _relatedObjectType;
        private object _relatedObject;

        #region Properties

        /// <summary>
        /// Represents whether the operation completed successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Status Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// More detailed Status Message
        /// </summary>
        public string MessageDetails { get; set; }

        /// <summary>
        /// Short representative Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Name of the Object that this status message refers to
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Int Id of the Object that this status message refers to
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Guid of the Object that this status message refers to
        /// </summary>
        public string ObjectGuid { get; set; }

        /// <summary>
        /// Returns TRUE if there is content in the 'Message' property
        /// </summary>
        public bool HasMessage
        {
            get
            {
                if (this.Message != string.Empty & this.Message != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Timestamp for start of message or operations
        /// </summary>
        /// <remarks>If not specified specifically, this will represent the time the StatusMessage object was created.</remarks>
        public DateTime TimestampStart { get; set; }

        /// <summary>
        /// Timestamp for end of operations
        /// </summary>
        /// <remarks>Must be specified specifically. Useful when attempting to time code operations</remarks>
        public DateTime? TimestampEnd { get; set; }

        /// <summary>
        /// Exceptions which occur can be attached here
        /// </summary>
        public Exception RelatedException { get; set; }

        /// <summary>
        /// Status Messages can be nested
        /// </summary>
        public List<StatusMessage> InnerStatuses { get; set; }

        /// <summary>
        /// Object which can be appended for additional information
        /// </summary>
        public object RelatedObject
        {
            get => _relatedObject;
            set
            {
                _relatedObject = value;

                if (value != null)
                {
                    _relatedObjectType = value.GetType();
                }
            }
        }

        /// <summary>
        /// Type of the Related Object
        /// </summary>
        public Type RelatedObjectType
        {
            get => _relatedObjectType;
        }

        #endregion


        #region Constructors

        public StatusMessage()
        {
            this.InnerStatuses = new List<StatusMessage>();
            this.TimestampStart = DateTime.Now;
        }

        public StatusMessage(DateTime StartTimestamp)
        {
            this.InnerStatuses = new List<StatusMessage>();
            this.TimestampStart = StartTimestamp;
        }

        public StatusMessage(bool WasSuccessful, DateTime? StartTimestamp = null)
        {
            this.InnerStatuses = new List<StatusMessage>();
            this.Success = WasSuccessful;

            if (StartTimestamp != null)
            {
                this.TimestampStart = (DateTime)StartTimestamp;
            }
            else
            {
                this.TimestampStart = DateTime.Now;
            }

        }

        public StatusMessage(bool WasSuccessful, string Msg, DateTime? StartTimestamp = null)
        {
            this.InnerStatuses = new List<StatusMessage>();
            this.Success = WasSuccessful;
            this.Message = Msg;

            if (StartTimestamp != null)
            {
                this.TimestampStart = (DateTime)StartTimestamp;
            }
            else
            {
                this.TimestampStart = DateTime.Now;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Duration between TimestampStart and TimestampEnd
        /// </summary>
        /// <returns></returns>
        public TimeSpan? TimeDuration()
        {
            if (TimestampEnd != null)
            {
                var duration = (DateTime)TimestampEnd - TimestampStart;
                return duration;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Converts StatusMessage into a HttpResponseMessage to return via a WebApi call, for instance.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage ToHttpResponse()
        {
            string json = JsonConvert.SerializeObject(this);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }
        /// <summary>
        /// Converts StatusMessage into a string appropriate for logging in a text file
        /// </summary>
        /// <param name="IndentLevel">Prepends dashes to indent the text for inner statuses (3 x IndentInterval)</param>
        /// <returns></returns>
        public string ToStringForLog(int IndentLevel = 0)
        {
            var sb = new StringBuilder();
            var indent = string.Concat(Enumerable.Repeat("---", IndentLevel)) + " ";

            sb.AppendLine(string.Format("{0}Success: {1}", indent, this.Success));

            if (this.HasMessage)
            {
                sb.AppendLine(string.Format("{0}Message: {1}", indent, this.Message));
                sb.AppendLine(this.MessageDetails);
            }

            if (this.InnerStatuses.Any())
            {
                sb.AppendLine(string.Format("{0}Inner Statuses:", indent));
                foreach (var message in this.InnerStatuses)
                {
                    sb.AppendLine(message.ToStringForLog(IndentLevel + 1));
                }
            }

            return sb.ToString();
        }

        #endregion


    }
}
