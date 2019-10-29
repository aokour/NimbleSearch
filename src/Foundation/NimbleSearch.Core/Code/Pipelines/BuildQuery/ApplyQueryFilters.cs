using NimbleSearch.Foundation.Abstractions.Pipelines.BuildQuery;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace NimbleSearch.Foundation.Core.Pipelines.BuildQuery
{
    public class ApplyQueryFilter: BuildQueryProcessor
    {
        public override void Process(BuildQueryArgs args)
        {
            var rules = args.TabItem?.QueryFilter;
            if (string.IsNullOrWhiteSpace(rules))
                return;

            var queryFilterPredicate = this.GetPredicatesFromRules(args.SearchContext, rules, args.TabItem?.Database);
            if(queryFilterPredicate != null)
            { 
                args.Query = args.Query.Filter(queryFilterPredicate);
            }
        }


        public Expression<Func<SearchResultItem, bool>> GetPredicatesFromRules(IProviderSearchContext context, string rawRules, Database db)
        {
            Assert.ArgumentNotNull(db, nameof(db));

            var expression = PredicateBuilder.True<SearchResultItem>();

            Sitecore.ContentSearch.Rules.QueryableRuleFactory ruleFactory = new Sitecore.ContentSearch.Rules.QueryableRuleFactory();
            var rules = ruleFactory.ParseRules<Sitecore.ContentSearch.Rules.QueryableRuleContext<SearchResultItem>>(db, rawRules);
            if (!rules.Rules.Any())
                return null;

            foreach (var rule in rules.Rules)
            {
                if (rule.Condition != null)
                {
                    var ruleContext = new Sitecore.ContentSearch.Rules.QueryableRuleContext<SearchResultItem>(context);
                    var stack = new RuleStack();
                    rule.Condition.Evaluate(ruleContext, stack);
                    rule.Execute(ruleContext);
                    if (stack.Any())
                    {
                        expression = ruleContext.Where;
                    }
                }
            }

            return expression;
        }
    }
}