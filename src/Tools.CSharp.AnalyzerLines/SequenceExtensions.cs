using System;
using System.Linq;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.AnalyzerLines
{
    public static class SequenceExtensions
    {
        #region private
        private static BaseContainerUnits _If(this BaseContainerUnits container, bool any, params Unit[] units)
        {
            var containerIfElsesUnits = container.LastAddedUnit as ContainerIfElsesUnits;
            if (containerIfElsesUnits == null)
            {
                containerIfElsesUnits = new ContainerIfElsesUnits();
                container.AddUnit(containerIfElsesUnits);
            }
            else
            {
                BaseContainerUnits ownerContainerIfElseUnits = containerIfElsesUnits;
                var nextContainer = containerIfElsesUnits;

                while (nextContainer != null)
                {
                    var lastAddedContainer = nextContainer.LastAddedUnit;
                    var containerListUnits = lastAddedContainer as ContainerListUnits;
                    if (containerListUnits != null)
                    {
                        ownerContainerIfElseUnits = containerListUnits;
                        nextContainer = ownerContainerIfElseUnits.LastAddedUnit as ContainerIfElsesUnits;
                    }
                    else
                    {
                        if (lastAddedContainer != null)
                        { throw new InvalidOperationException(); }
                    }
                }

                containerIfElsesUnits = new ContainerIfElsesUnits();
                ownerContainerIfElseUnits.AddUnit(containerIfElsesUnits);
            }

            var newContainer = any ? new ContainerAnyUnits() : (BaseContainerUnits)(new ContainerListUnits());

            foreach (var unit in units)
            { newContainer.AddUnit(unit); }

            containerIfElsesUnits.AddUnit(newContainer);

            return newContainer;
        }
        private static BaseContainerUnits _Else(this BaseContainerUnits container, bool any, params Unit[] units)
        {
            var containerIfElsesUnits = container.LastAddedUnit as ContainerIfElsesUnits;
            if (containerIfElsesUnits == null)
            {
                containerIfElsesUnits = new ContainerIfElsesUnits();
                container.AddUnit(containerIfElsesUnits);
            }
            else
            {
                var ownerContainerIfElseUnits = containerIfElsesUnits;
                var nextContainer = containerIfElsesUnits;

                while (nextContainer != null)
                {
                    var lastAddedContainer = nextContainer.LastAddedUnit;
                    var containerListUnits = lastAddedContainer as ContainerListUnits;
                    if (containerListUnits != null)
                    {
                        ownerContainerIfElseUnits = containerIfElsesUnits;
                        nextContainer = containerListUnits.LastAddedUnit as ContainerIfElsesUnits;
                    }
                    else
                    {
                        if (lastAddedContainer != null)
                        { throw new InvalidOperationException(); }
                    }
                }

                containerIfElsesUnits = ownerContainerIfElseUnits;
            }

            var newContainer = any ? new ContainerAnyUnits() : (BaseContainerUnits)(new ContainerListUnits());

            foreach (var unit in units)
            { newContainer.AddUnit(unit); }

            containerIfElsesUnits.AddUnit(newContainer);

            return newContainer;
        }
        #endregion
        //---------------------------------------------------------------------
        public static TUnit AddIgnoreSymbol<TUnit>(this TUnit unit, char ignoreSymbol)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.IgnoreSymbols.Add(ignoreSymbol);
            return unit;
        }
        public static TUnit AddIgnoreSymbols<TUnit>(this TUnit unit, params char[] ignoreSymbols)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.IgnoreSymbols.Add(ignoreSymbols);
            return unit;
        }
        public static TUnit AddIgnoreUnit<TUnit>(this TUnit unit, SymbolUnit ignoreUnit)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }
            if (ignoreUnit == null)
            { throw new ArgumentNullException(nameof(ignoreUnit)); }

            unit.IgnoreSymbols.Add(ignoreUnit.OriginalValue);
            return unit;
        }
        public static TUnit AddIgnoreUnits<TUnit>(this TUnit unit, params SymbolUnit[] ignoreUnits)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            foreach (var ignoreUnit in ignoreUnits.Where(ignoreUnit => ignoreUnit != null))
            { unit.IgnoreSymbols.Add(ignoreUnit.OriginalValue); }

            return unit;
        }
        //---------------------------------------------------------------------
        public static TUnit RemoveIgnoreSymbol<TUnit>(this TUnit unit, char ignoreSymbol)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.IgnoreSymbols.Remove(ignoreSymbol);
            return unit;
        }
        public static TUnit RemoveIgnoreSymbols<TUnit>(this TUnit unit, params char[] ignoreSymbols)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.IgnoreSymbols.RemoveRange(ignoreSymbols);
            return unit;
        }
        public static TUnit RemoveIgnoreUnit<TUnit>(this TUnit unit, SymbolUnit ignoreUnit)
           where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }
            if (ignoreUnit == null)
            { throw new ArgumentNullException(nameof(ignoreUnit)); }

            unit.IgnoreSymbols.Remove(ignoreUnit.OriginalValue);
            return unit;
        }
        public static TUnit RemoveIgnoreUnits<TUnit>(this TUnit unit, params SymbolUnit[] ignoreUnits)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            foreach (var ignoreUnit in ignoreUnits.Where(ignoreUnit => ignoreUnit != null))
            { unit.IgnoreSymbols.Remove(ignoreUnit.OriginalValue); }

            return unit;
        }
        //---------------------------------------------------------------------
        public static TUnit ClearIgnoreSymbols<TUnit>(this TUnit unit, bool forever = false)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.IgnoreSymbols.Clear(forever);
            return unit;
        }
        //---------------------------------------------------------------------
        public static TUnit DecodeLineIgnoreCaseSymbols<TUnit>(this TUnit unit, bool ignoreCaseSymbols)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.DecodeLineIgnoreCaseSymbols = ignoreCaseSymbols;
            return unit;
        }
        //---------------------------------------------------------------------
        public static TContainerUnits Add<TContainerUnits>(this TContainerUnits container, params Unit[] units)
           where TContainerUnits : BaseContainerUnits
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            foreach (var unit in units)
            { container.AddUnit(unit); }

            return container;
        }
        public static TContainerUnits Add<TContainerUnits>(this TContainerUnits container, Unit unit)
           where TContainerUnits : BaseContainerUnits
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            container.AddUnit(unit);

            return container;
        }

        public static ContainerAnyUnits Add(this ContainerAnyUnits container, int aliveCount, params Unit[] units)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            foreach (var unit in units)
            { container.AddUnit(unit, aliveCount); }

            return container;
        }
        public static ContainerAnyUnits Add(this ContainerAnyUnits container, int aliveCount, Unit unit)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            container.AddUnit(unit, aliveCount);

            return container;
        }
        //---------------------------------------------------------------------
        public static TContainerUnits Any<TContainerUnits>(this TContainerUnits container, params Unit[] units)
            where TContainerUnits : BaseContainerUnits
        {
            return container.Any(1, units);
        }
        public static TContainerUnits Any<TContainerUnits>(this TContainerUnits container, int aliveCount, params Unit[] units)
            where TContainerUnits : BaseContainerUnits
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            var containerAnyUnits = new ContainerAnyUnits();

            foreach (var unit in units)
            { containerAnyUnits.AddUnit(unit, aliveCount); }

            container.AddUnit(containerAnyUnits);

            return container;
        }
        //---------------------------------------------------------------------
        public static BaseContainerUnits If(this Sequence container, params Unit[] units)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            return container._If(false, units);
        }
        public static BaseContainerUnits Else(this Sequence container, params Unit[] units)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            return container._Else(false, units);
        }

        public static BaseContainerUnits IfAny(this Sequence container, params Unit[] units)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            return container._If(true, units);
        }
        public static BaseContainerUnits ElseAny(this Sequence container, params Unit[] units)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            return container._Else(true, units);
        }
        //---------------------------------------------------------------------
        public static ContainerListUnits CreateListContainer(this BaseContainerUnits container)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            return new ContainerListUnits();
        }
        public static ContainerListUnits CreateListContainer(this BaseContainerUnits container, params Unit[] units)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            var newContainer = new ContainerListUnits();
            return newContainer.Add(units);
        }

        public static ContainerAnyUnits CreateAnyContainer(this BaseContainerUnits container)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            return new ContainerAnyUnits();
        }
        public static ContainerAnyUnits CreateAnyContainer(this BaseContainerUnits container, params Unit[] units)
        {
            return container.CreateAnyContainer(1, units);
        }
        public static ContainerAnyUnits CreateAnyContainer(this BaseContainerUnits container, int aliveCount, params Unit[] units)
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            var newContainer = new ContainerAnyUnits();
            return newContainer.Add(aliveCount, units);
        }
        //---------------------------------------------------------------------
        public static TContainerUnits Error<TContainerUnits>(this TContainerUnits container, Action<UnitError> error)
            where TContainerUnits : BaseContainerUnits
        {
            if (container == null)
            { throw new ArgumentNullException(nameof(container)); }

            container.SetRaiseError(error);
            return container;
        }
        //---------------------------------------------------------------------
        public static TUnit Action<TUnit>(this TUnit unit, Action action)
            where TUnit : Unit
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.AddAction(action);
            return unit;
        }

        public static TUnit Action<TUnit>(this TUnit unit, Action<TUnit> action)
            where TUnit : Unit<TUnit>
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.AddAction(action);
            return unit;
        }

        public static TUnit Action<TUnit>(this TUnit unit, Action<string> action)
            where TUnit : BaseValueUnit<TUnit>
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            unit.AddAction(action);
            return unit;
        }
        //---------------------------------------------------------------------
    }
}