using System.Collections.ObjectModel;

namespace Tools.CSharp.ViewModels
{
    //---------------------------------------------------------------------
    public interface IBaseViewModelValidatorError<in TViewModel, TError> : IBaseViewModelValidator<TViewModel, PropertyValidationResult<TError>>
        where TViewModel : IBaseViewModel
        where TError : struct
    { }
    //---------------------------------------------------------------------
    public interface IBaseViewModelValidator<in TViewModel> : IBaseViewModelValidator<TViewModel, PropertyValidationResult>
       where TViewModel : IBaseViewModel
    { }
    //---------------------------------------------------------------------
    public interface IBaseViewModelValidator<in TViewModel, out TPropertyValidationResult>
        where TViewModel : IBaseViewModel
        where TPropertyValidationResult : PropertyValidationResult
    {
        //---------------------------------------------------------------------
        ReadOnlyCollection<string> PropertiesToValidation { get; }
        //---------------------------------------------------------------------
        bool IsPropertyForValidationAvailable(string propertyName);
        //---------------------------------------------------------------------
        TPropertyValidationResult GetNotValidValidationResultForViewModel(TViewModel viewModel);
        TPropertyValidationResult GetNotValidValidationResultForViewModel(TViewModel viewModel, bool isAllMessageInvalidity);
        //---------------------------------------------------------------------
        TPropertyValidationResult GetValidationResultForProperty(TViewModel viewModel, string propertyName);
        TPropertyValidationResult GetValidationResultForProperty(TViewModel viewModel, string propertyName, bool isAllMessageInvalidity);
        //---------------------------------------------------------------------
    }
    //---------------------------------------------------------------------
}