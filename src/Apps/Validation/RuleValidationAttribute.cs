using System;
using Yesolm.DevOps.Models;

namespace Yesolm.DevOps.Validation
{
    public class RuleValidationAttribute : Attribute
    {
        public RuleValidationAttribute(Rule rule) => Rule = rule;
        public RuleValidationAttribute(Rule rule, Type type) : this(rule) => Type = type;
        public Rule Rule { get; }
        public Type Type { get; private set; }
    }
}
