using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tools.CSharp.ViewModels
{
    //---------------------------------------------------------------------
    public abstract class BaseViewModelValidatorError<TViewModel, TError> : BaseViewModelValidator<TViewModel, PropertyValidationResult<TError>>, IBaseViewModelValidatorError<TViewModel, TError> 
        where TViewModel : IBaseViewModel
        where TError : struct
    {
        #region protected
        protected override PropertyValidationResult<TError> CreateValidationResultForProperty(TViewModel viewModel, string propertyName, bool isAllMessageInvalidity)
        {
            return new PropertyValidationResult<TError>(propertyName);
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public abstract class BaseViewModelValidator<TViewModel> : BaseViewModelValidator<TViewModel, PropertyValidationResult>, IBaseViewModelValidator<TViewModel> 
        where TViewModel : IBaseViewModel
    {
        #region protected
        protected override PropertyValidationResult CreateValidationResultForProperty(TViewModel viewModel, string propertyName, bool isAllMessageInvalidity)
        {
            return new PropertyValidationResult(propertyName);
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public abstract class BaseViewModelValidator<TViewModel, TPropertyValidationResult> : IBaseViewModelValidator<TViewModel, TPropertyValidationResult>
        where TViewModel : IBaseViewModel
        where TPropertyValidationResult : PropertyValidationResult
    {
        #region private
        private readonly List<string> _propertiesToValidation = new List<string>();
        #endregion
        #region protected
        protected void AddPropertiesToValidation(params string[] propertiesName)
        {
            if (propertiesName == null)
            { throw new ArgumentNullException(nameof(propertiesName)); }

            foreach(var propertyName in propertiesName)
            { AddPropertyToValidation(propertyName); }
        }
        protected bool AddPropertyToValidation(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            { throw new ArgumentNullException(nameof(propertyName)); }

            if (!_propertiesToValidation.Contains(propertyName))
            {
                _propertiesToValidation.Add(propertyName);
                return true;
            }

            return false;
        }
        //---------------------------------------------------------------------
        protected abstract TPropertyValidationResult CreateValidationResultForProperty(TViewModel viewModel, string propertyName, bool isAllMessageInvalidity);
        #endregion
        //---------------------------------------------------------------------
        public ReadOnlyCollection<string> PropertiesToValidation
        {
            get { return _propertiesToValidation.AsReadOnly(); }
        }
        //---------------------------------------------------------------------
        public bool IsPropertyForValidationAvailable(string propertyName)
        {
            return _propertiesToValidation.IndexOf(propertyName) != -1;
        }
        //---------------------------------------------------------------------
        public TPropertyValidationResult GetNotValidValidationResultForViewModel(TViewModel viewModel)
        {
            return GetNotValidValidationResultForViewModel(viewModel, true);
        }
        public TPropertyValidationResult GetNotValidValidationResultForViewModel(TViewModel viewModel, bool isAllMessageInvalidity)
        {
            if (viewModel == null)
            { throw new ArgumentNullException(nameof(viewModel)); }

            return _propertiesToValidation
                .Select(validationProperty => CreateValidationResultForProperty(viewModel, validationProperty, isAllMessageInvalidity))
                .FirstOrDefault(validationResultForProperty => validationResultForProperty != null && !validationResultForProperty.IsValid);
        }
        //---------------------------------------------------------------------
        public TPropertyValidationResult GetValidationResultForProperty(TViewModel viewModel, string propertyName)
        {
            return GetValidationResultForProperty(viewModel, propertyName, true);
        }
        public TPropertyValidationResult GetValidationResultForProperty(TViewModel viewModel, string propertyName, bool isAllMessageInvalidity)
        {
            if (viewModel == null)
            { throw new ArgumentNullException(nameof(viewModel)); }

            return IsPropertyForValidationAvailable(propertyName) ? CreateValidationResultForProperty(viewModel, propertyName, isAllMessageInvalidity) : null;
        }
    }
    //---------------------------------------------------------------------
}