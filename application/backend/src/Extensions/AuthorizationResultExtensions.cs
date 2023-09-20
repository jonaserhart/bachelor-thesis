using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace backend.Extensions;

public static class AuthorizationResultExtensions
{
    public static string FailureReasonsToString(this AuthorizationResult authorizationResult)
    {
        var message = new StringBuilder();
        if (authorizationResult.Failure != null)
        {
            foreach (var failureReason in authorizationResult.Failure.FailureReasons)
            {
                message.Append(failureReason.Message).AppendLine();
            }
        }
        return message.ToString();
    }
}