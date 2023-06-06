using backend.Model.Analysis.Expressions;
using backend.Model.Analysis.WorkItems;
using backend.Model.Enum;
using backend.Test.Helpers;

namespace backend.Test.Model.Analysis.Expressions;

[TestFixture]
public class EvaluateExpressionsTests
{


    [Test]
    public void EvaluateSumExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var expected = 25;
        var workItems = TestServices.GenerateWorkItemsWithField(fieldName, WorkItemValueType.Number, "2", "3", "7", "13");


        var expression = new SumExpression { FieldExpression = new FieldExpression { Field = fieldName } };

        var actual = expression.Evaluate(workItems);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateAvgExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var expected = 6.25;
        var workItems = TestServices.GenerateWorkItemsWithField(fieldName, WorkItemValueType.Number, "2", "3", "7", "13");


        var expression = new AvgExpression { FieldExpression = new FieldExpression { Field = fieldName } };

        var actual = expression.Evaluate(workItems);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateMaxExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var expected = 13;
        var workItems = TestServices.GenerateWorkItemsWithField(fieldName, WorkItemValueType.Number, "2", "3", "7", "13");


        var expression = new MaxExpression { FieldExpression = new FieldExpression { Field = fieldName } };

        var actual = expression.Evaluate(workItems);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateMinExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var expected = 2;
        var workItems = TestServices.GenerateWorkItemsWithField(fieldName, WorkItemValueType.Number, "2", "3", "7", "13");


        var expression = new MinExpression { FieldExpression = new FieldExpression { Field = fieldName } };

        var actual = expression.Evaluate(workItems);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateCountIfExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.State";
        var expected = 3;
        var workItems = TestServices.GenerateWorkItemsWithField(fieldName, WorkItemValueType.String, "Closed", "New", "Closed", "Closed");


        var expression = new CountIfExpression { Field = fieldName, Operator = "!=", Value = "New" };

        var actual = expression.Evaluate(workItems);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EvaluateDivExpression_GiveWorkItems_EvaluatesExpression()
    {
        var fieldName = "System.OriginalEstimate";
        var expected = 0.892;
        var workItems = TestServices.GenerateWorkItemsWithField(fieldName, WorkItemValueType.Number, "2", "3", "7", "13");

        var secondFieldName = "System.Estimate";
        workItems.AddFieldToWorkItems(secondFieldName, WorkItemValueType.Number, "3", "2", "9", "14");

        var expression = new DivExpression
        {
            Left = new SumExpression { FieldExpression = new FieldExpression { Field = fieldName } },
            Right = new SumExpression { FieldExpression = new FieldExpression { Field = secondFieldName } }
        };

        var actual = expression.Evaluate(workItems);

        Assert.That(actual, Is.InRange(expected, expected + 0.001));
    }
}