﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Havit.Blazor.Components.Web.Bootstrap.Forms
{
	public class HxInputDate<TValue> : HxInputBase<TValue>
	{
		private const string DateFormat = "yyyy-MM-dd"; // Compatible with HTML date inputs

		// TODO
		/// </summary>
		[Parameter] public string ParsingErrorMessage { get; set; }

		protected override void BuildRenderInput(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "input");
			
			BuildRenderInput_AddCommonAttributes(builder, "date");

			builder.AddAttribute(1000, "value", FormatValueAsString(Value));
			builder.AddAttribute(1001, "onchange", EventCallback.Factory.CreateBinder<string>(this, value => CurrentValueAsString = value, CurrentValueAsString));
			
			builder.CloseElement();
		}

		/// <inheritdoc />
		protected override string FormatValueAsString(TValue value)
		{
			switch (value)
			{
				case DateTime dateTimeValue:
					return BindConverter.FormatValue(dateTimeValue, DateFormat, CultureInfo.InvariantCulture);
				case DateTimeOffset dateTimeOffsetValue:
					return BindConverter.FormatValue(dateTimeOffsetValue, DateFormat, CultureInfo.InvariantCulture);
				default:
					return string.Empty; // Handles null for Nullable<DateTime>, etc.
			}
		}

		/// <inheritdoc />
		protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
		{
			// Unwrap nullable types. We don't have to deal with receiving empty values for nullable
			// types here, because the underlying InputBase already covers that.
			var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

			bool success;
			if (targetType == typeof(DateTime))
			{
				success = TryParseDateTime(value, out result);
			}
			else if (targetType == typeof(DateTimeOffset))
			{
				success = TryParseDateTimeOffset(value, out result);
			}
			else
			{
				throw new InvalidOperationException($"The type '{targetType}' is not a supported date type.");
			}

			if (success)
			{
				validationErrorMessage = null;
				return true;
			}
			else
			{
				validationErrorMessage = string.Format(ParsingErrorMessage, FieldIdentifier.FieldName);
				return false;
			}
		}

		private static bool TryParseDateTime(string value, out TValue result)
		{
			var success = BindConverter.TryConvertToDateTime(value, CultureInfo.InvariantCulture, DateFormat, out var parsedValue);
			if (success)
			{
				result = (TValue)(object)parsedValue;
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		private static bool TryParseDateTimeOffset(string value, out TValue result)
		{
			var success = BindConverter.TryConvertToDateTimeOffset(value, CultureInfo.InvariantCulture, DateFormat, out var parsedValue);
			if (success)
			{
				result = (TValue)(object)parsedValue;
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}
	}
}