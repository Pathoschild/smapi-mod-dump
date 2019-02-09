using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Json.Serialization {
    public class DescriptiveJsonWriter : JsonWriter {
        private readonly TextWriter _writer;
        private int _indent;
        private string _propertyName;
        private string _propertyComment;
        private readonly Stack<bool> _needsComma = new Stack<bool>();
        private bool _needsNewLine;

        /// <summary>Whether to make the JSON as small as possible</summary>
        public bool Minify { get; set; } = false;

        /// <summary>The indentation to use</summary>
        public string Indentation { get; set; } = "    ";

        /// <summary>The quote character used to denote strings</summary>
        public char QuoteChar { get; set; } = '"';

        public DescriptiveJsonWriter(TextWriter textWriter) {
            this._writer = textWriter;
            this._needsComma.Push(false);
        }

        #region Overrides
        public override void Flush() {
            this._writer.Flush();
        }

        public override void WritePropertyName(string name) {
            this._propertyName = name;
        }

        public void WritePropertyComment(string comment) {
            this._propertyComment = comment;
        }

        public void Indent() {
            if (this.Minify)
                return;

            this._writer.Write(this.Indentation.Repeat(this._indent));
        }

        public void NextLine() {
            this._needsNewLine = false;
            if (this.Minify)
                return;

            this._writer.WriteLine();
            this.Indent();
        }

        private void WriteComma() {
            this._writer.Write(',');
            if (!this.Minify && this._propertyComment != null) {
                this._writer.WriteLine();
                this.Indent();
            }
        }

        public override void WriteComment(string text) {
            if (this._needsNewLine)
                this.NextLine();

            this._writer.Write($"/*{text.Replace("*/", "* /")}*/");
        }

        public override void WriteStartObject() {
            // Put a comma if needed
            if (this._needsComma.Pop()) {
                this.WriteComma();
            }

            // Go to the next line if needed
            if (this._needsNewLine)
                this.NextLine();

            // Keep track of whether this will need a comma or not
            this._needsComma.Push(true);

            // If this is a property, write the comment and name
            this.BeginProperty();

            // Begin object
            this._writer.Write('{');
            this._needsComma.Push(false);
            this._indent++;
            this._needsNewLine = true;
        }

        public override void WriteStartArray() {
            // Put a comma if needed
            if (this._needsComma.Pop()) {
                this.WriteComma();
            }

            // Go to the next line if needed
            if (this._needsNewLine)
                this.NextLine();

            // Keep track of whether this will need a comma or not
            this._needsComma.Push(true);

            // If this is a property, write the comment and name
            this.BeginProperty();

            // Begin array
            this._writer.Write('[');
            this._needsComma.Push(false);
            this._indent++;
            this._needsNewLine = true;
        }

        public override void WriteEndArray() {
            this._indent--;
            if (this._needsNewLine)
                this.NextLine();

            this._writer.Write(']');
            this._needsComma.Pop();

            this._needsNewLine = true;
        }

        public override void WriteEndObject() {
            this._indent--;
            if (this._needsNewLine)
                this.NextLine();

            this._writer.Write('}');
            this._needsComma.Pop();

            this._needsNewLine = true;
        }

        #region Value Writers
        public override void WriteNull() {
            this.WriteValueToken(JTokenType.Null, "null");
        }

        public override void WriteValue(bool value) {
            this.WriteValueToken(JTokenType.Boolean, value ? "true" : "false");
        }

        public override void WriteValue(sbyte value) {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(byte value) {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(short value) {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(ushort value) {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(int value) {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(uint value) {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(long value) {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(ulong value) {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(float value) {
            this.WriteValueToken(JTokenType.Float, value.ToString(this.Culture));
        }

        public override void WriteValue(double value) {
            this.WriteValueToken(JTokenType.Float, value.ToString(this.Culture));
        }

        public override void WriteValue(decimal value) {
            this.WriteValueToken(JTokenType.Float, value.ToString(this.Culture));
        }

        public override void WriteValue(char value) {
            this.WriteValueToken(JTokenType.String, this.ToJsonString(value.ToString()));
        }

        public override void WriteValue(string value) {
            this.WriteValueToken(JTokenType.String, this.ToJsonString(value));
        }

        public override void WriteValue(DateTime value) {
            if (string.IsNullOrEmpty(this.DateFormatString)) {
                // ISO-8601
                this._writer.Write(value.ToString("O"));
            } else {
                // Custom date format string
                this.WriteValueToken(JTokenType.Date, $"{this.QuoteChar}{value.ToUniversalTime().ToString(this.DateFormatString, this.Culture)}{this.QuoteChar}");
            }
        }

        public override void WriteValue(DateTimeOffset value) {
            if (string.IsNullOrEmpty(this.DateFormatString)) {
                // ISO-8601
                this.WriteValueToken(JTokenType.Date, $"{this.QuoteChar}{value:O}{this.QuoteChar}");
            } else {
                // Custom date format string
                this.WriteValueToken(JTokenType.Date, $"{this.QuoteChar}{value.UtcDateTime.ToString(this.DateFormatString, this.Culture)}{this.QuoteChar}");
            }
        }

        public override void WriteValue(Guid value) {
            this.WriteValueToken(JTokenType.String, $"{this.QuoteChar}{value.ToString("D", this.Culture)}{this.QuoteChar}");
        }

        public override void WriteValue(TimeSpan value) {
            this.WriteValueToken(JTokenType.TimeSpan, $"{this.QuoteChar}{value.ToString(null, this.Culture)}{this.QuoteChar}");
        }

        public override void WriteValue(Uri value) {
            if (value == null) {
                this.WriteNull();
            } else {
                this.WriteValue(value.OriginalString);
            }
        }

        public override void WriteValue(byte[] value) {
            if (value == null) {
                this.WriteNull();
            } else {
                this.WriteValue(Convert.ToBase64String(value));
            }
        }

        #region Nullables
        public override void WriteValue(int? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(uint? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(long? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(ulong? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(float? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(double? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(bool? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(short? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(ushort? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(char? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(byte? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(sbyte? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(decimal? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(DateTime? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(DateTimeOffset? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(Guid? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }

        public override void WriteValue(TimeSpan? value) {
            if (value.HasValue)
                this.WriteValue(value.Value);
            this.WriteNull();
        }
        #endregion
        #endregion
        #endregion

        protected void WriteValueToken(JTokenType type, string value) {
            // Put a comma if needed
            if (this._needsComma.Pop()) {
                this.WriteComma();
            }

            // Go to the next line if needed
            if (this._needsNewLine)
                this.NextLine();

            // If this is a property, write the comment and name
            this.BeginProperty();

            // Write the value
            this._writer.Write(value);

            // Indicate that a value was written and this is the end of the line
            this._needsNewLine = true;
            this._needsComma.Push(true);
        }

        protected void BeginProperty() {
            if (this._propertyName == null)
                return;

            // Write property comment
            if (this._propertyComment != null && !this.Minify) {
                this._writer.Write("/* ");
                this._writer.Write(this._propertyComment.Replace("*/", "* /"));
                this._writer.Write(" */");
                this.NextLine();
            }

            // Write property name
            this._writer.Write(this.ToJsonString(this._propertyName));

            // Write delimiter
            this._writer.Write(":");
            if (!this.Minify) {
                this._writer.Write(' ');
            }

            // Reset property name and comment
            this._propertyName = null;
            this._propertyComment = null;
        }

        protected string ToJsonString(string str) {
            return JsonConvert.ToString(str);
        }
    }
}