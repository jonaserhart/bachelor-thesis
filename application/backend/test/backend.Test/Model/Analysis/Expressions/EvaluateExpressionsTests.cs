using System.Net.Mail;
using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Analysis.KPIs;
using backend.Model.Enum;

namespace backend.Test.Model.Analysis.Expressions;

[TestFixture]
public class EvaluateExpressionsTests
{

    [Test]
    public void EvaluateSumExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var query = "q1";
        var expected = 25;
        var queryResult = new QueryResult
        {
            Type = QueryReturnType.ObjectList,
            Value = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {fieldName, 12},
                            {"Other.Field", "Othervalue"},
                        },
                        new Dictionary<string, object>
                        {
                            {fieldName, 13},
                            {"Other.Field", "Othervalue"},
                        }
                    }
        };

        var expression = new SumExpression { Field = fieldName, Type = ExpressionType.Sum, QueryId = query };

        var actual = expression.Evaluate(new Dictionary<string, QueryResult> { { query, queryResult } });

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateAvgExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var query = "q1";
        var expected = 15;
        var queryResult = new QueryResult
        {
            Type = QueryReturnType.ObjectList,
            Value = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {fieldName, 20},
                            {"Other.Field", "Othervalue"},
                        },
                        new Dictionary<string, object>
                        {
                            {fieldName, 10},
                            {"Other.Field", "Othervalue"},
                        }
                    }
        };

        var expression = new AvgExpression { Field = fieldName, QueryId = query, Type = ExpressionType.Avg };

        var actual = expression.Evaluate(new Dictionary<string, QueryResult> { { query, queryResult } });

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateMaxExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var query = "q1";
        var expected = 20;
        var queryResult = new QueryResult
        {
            Type = QueryReturnType.ObjectList,
            Value = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {fieldName, 20},
                            {"Other.Field", "Othervalue"},
                        },
                        new Dictionary<string, object>
                        {
                            {fieldName, 10},
                            {"Other.Field", "Othervalue"},
                        }
                    }
        };

        var expression = new MaxExpression { Field = fieldName, QueryId = query, Type = ExpressionType.Max };

        var actual = expression.Evaluate(new Dictionary<string, QueryResult> { { query, queryResult } });

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateCountIfMultipleConditionsExpression_GiveWorkItems_EvaluatesExpression()
    {
        var query = "q1";
        var expected = 2;
        var queryResult = new QueryResult
        {
            Type = QueryReturnType.ObjectList,
            Value = new List<Dictionary<string, object>>
                {
                    new() {
                        {"Field1", 20},
                        {"Field2", "Value1"},
                    },
                    new() {
                        {"Field1", 10},
                        {"Field2", "Value2"},
                    },
                    new() {
                        {"Field1", 10},
                        {"Field2", "Value1"},
                    }
                }
        };

        var conditions = new List<Condition>
            {
                new Condition { Field = "Field1", Operator = CountIfOperator.IsMore, CompareValue = "9" },
                new Condition { Field = "Field2", Operator = CountIfOperator.IsEqual, CompareValue = "Value1" }
            };

        var expression = new CountIfMultipleExpression
        {
            Conditions = conditions,
            Connection = ConditionConnection.All,
            QueryId = query,
            Type = ExpressionType.CountIfMultiple
        };

        var actual = expression.Evaluate(new Dictionary<string, QueryResult> { { query, queryResult } });

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateCountIfExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var query1 = "q1";
        var query2 = "q2";
        var expected = 0.92;
        var queryResult1 = new QueryResult
        {
            Type = QueryReturnType.ObjectList,
            Value = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {fieldName, 20},
                            {"Other.Field", "Othervalue"},
                        },
                        new Dictionary<string, object>
                        {
                            {fieldName, 10},
                            {"Other.Field", "Othervalue"},
                        }
                    }
        };
        var queryResult2 = new QueryResult
        {
            Type = QueryReturnType.ObjectList,
            Value = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {fieldName, 2.5},
                            {"Other.Field", "Othervalue"},
                        },
                        new Dictionary<string, object>
                        {
                            {fieldName, 30},
                            {"Other.Field", "Othervalue"},
                        }
                    }
        };

        var left = new KPI
        {
            Expression = new SumExpression { Field = fieldName, Type = ExpressionType.Sum, QueryId = query1 }
        };

        var right = new KPI
        {
            Expression = new SumExpression { Field = fieldName, Type = ExpressionType.Sum, QueryId = query2 }
        };

        var expression = new DivExpression { Left = left, Right = right, Type = ExpressionType.Div };

        var actual = expression.EvaluateMathExpression(new Dictionary<string, QueryResult> { { query1, queryResult1 }, { query2, queryResult2 } });

        Assert.That(Math.Abs(actual - expected), Is.LessThanOrEqualTo(0.01));
    }

}