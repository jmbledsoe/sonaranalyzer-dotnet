﻿namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;
    using Nancy.Validation.Rules;
    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    /// <summary>
    /// Adapter between the Fluent Validation <see cref="NotEqualValidator"/> and the Nancy validation rules.
    /// </summary>
    public class NotEqualAdapter : AdapterBase
    {
        /// <summary>
        /// Gets whether or not the adapter can handle the provided <see cref="IPropertyValidator"/> instance.
        /// </summary>
        /// <param name="validator">The <see cref="IPropertyValidator"/> instance to check for compatibility with the adapter.</param>
        /// <returns><see langword="true" /> if the adapter can handle the validator, otherwise <see langword="false" />.</returns>
        public override bool CanHandle(IPropertyValidator validator)
        {
            return validator is NotEqualValidator;
        }

        /// <summary>
        /// Get the <see cref="ModelValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules(PropertyRule rule, IPropertyValidator validator)
        {
            yield return new ComparisonValidationRule(
                base.FormatMessage(rule, validator),
                base.GetMemberNames(rule),
                ComparisonOperator.NotEqual,
                ((NotEqualValidator)validator).ValueToCompare);
        }
    }
}