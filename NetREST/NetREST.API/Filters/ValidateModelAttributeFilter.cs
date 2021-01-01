using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NetREST.Common.Errors;
using NetREST.Common.Response;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace NetREST.API.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
	    public override void OnActionExecuting(ActionExecutingContext context)
	    {
		    var requestParameters = GetValidationArguments(context).ToArray();

		    if (requestParameters.Any(x => x.IsRequired && x.Value == null))
		    {
			    context.Result = GetBadRequestActionResult();
			    return;
		    }

		    var validationResult = GetValidationResult(context, requestParameters.Where(x => x.Value != null))
			    .Where(x => !x.IsValid).ToArray();
		    if (validationResult.Any())
		    {
			    context.Result = GetUnprocessableEntityActionResult(validationResult.SelectMany(x => x.Errors));
		    }
	    }

	    protected virtual IEnumerable<ValidatorArgument> GetValidationArguments(ActionExecutingContext context) => context.ActionDescriptor.Parameters
		    .OfType<ControllerParameterDescriptor>()
		    .Select(x => new ValidatorArgument
		    {
			    Name = x.Name,
			    Value = context.ActionArguments.FirstOrDefault(y => y.Key == x.Name).Value,
			    ValueType = x.ParameterType,
			    // IsRequired = x.ParameterInfo.CustomAttributes.Any(y => y.AttributeType == typeof(RequiredAttribute))
		    });
	    
	    protected ValidationResult Validate(IValidatorFactory factory, Type modelType, object model)
	    {
		    IValidationContext context = new ValidationContext<object>(model);
		    var result = factory.GetValidator(modelType)?.Validate(context);
		    return result;
	    }
	    
	    protected virtual IEnumerable<ValidationResult> GetValidationResult(ActionExecutingContext context,
		    IEnumerable<ValidatorArgument> arguments)
	    {
		    var validatorFactory = context
			    .HttpContext
			    .RequestServices
			    .GetService<IValidatorFactory>();

		    var result = arguments
			    .Where(argement => argement?.ValueType != null)
			    .Select(argument => Validate(validatorFactory, argument.ValueType, argument.Value))
			    .Where(r => r != null)
			    .ToArray();

		    return result;
	    }

	    protected virtual IActionResult GetBadRequestActionResult()
	    {
		    return new ObjectResult(new ErrorResponse
		    {
			    ErrorCode = ErrorCodes.Common.BadParameters,
			    ErrorMessage = ErrorMessages.Common.BadParameters,
			    Errors = new []
			    {
				    new Error
				    {
					    Code = ErrorCodes.Common.BadParameters,
					    Message = "Invalid type format of one or many model fields"
				    }, 
			    }
		    })
		    {
			    StatusCode = 400
		    };
	    }
	    
	    protected virtual IActionResult GetUnprocessableEntityActionResult(IEnumerable<ValidationFailure> errors)
	    {
		    return new ObjectResult(
			    new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.Common.BadParameters,
				    ErrorMessage = ErrorMessages.Common.UnprocessableEntity,
				    Errors = errors.Select(GetError).ToArray(),
			    })
		    {
			    StatusCode = (int) HttpStatusCode.BadRequest,
		    };
	    }
	    
	    protected Error GetError(ValidationFailure error)
	    {
		    return new Error
		    {
			    Code = error.ErrorCode,
			    Message = error.ErrorMessage,
			    Field = error.PropertyName,
		    };
	    }

	    protected class ValidatorArgument
	    {
		    public string Name { get; set; }
		    public object Value { get; set; }
		    public Type ValueType { get; set; }
		    public bool IsRequired { get; set; }
	    }
	    
		// public override void OnActionExecuting(ActionExecutingContext context)
		// {
		// 	if (!context.ModelState.IsValid)
  //           {
  //               var errors = context.ModelState
  //                   .SelectMany(
  //                       o => o.Value.Errors,
  //                       (a, b) => new ValidationError { Key = a.Key, Error = b })
  //                   .Select(GetError)
  //                   .ToArray();
  //               context.Result = ValidationErrorResponse(errors);
  //           }
		// }
	 //
  //       protected virtual IActionResult ValidationErrorResponse(IEnumerable<Error> errors)
  //       {
  //           return new ObjectResult(new ErrorResponse()
  //           {
  //               ErrorCode = "bad_parameters",
  //               ErrorMessage = "Invalid input parameters",
  //               Errors = errors,
  //           })
  //           {
  //               StatusCode = (int) HttpStatusCode.BadRequest,
  //           };
  //       }
  //
  //       protected virtual Error GetError(ValidationError error)
  //       {
  //           return new Error
  //           {
  //               Code = "Validation error",
  //               Message = error.Error.ErrorMessage,
  //               Field = error.Key,
  //           };
  //       }
  //       
  //       public override void OnActionExecuted(ActionExecutedContext context)
  //       {
  //       }
  //       
  //       protected class ValidationError
  //       {
  //           public string Key { get; set; }
  //           public ModelError Error { get; set; }
  //       }
	}
}
