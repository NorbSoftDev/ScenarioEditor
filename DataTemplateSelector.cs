using NorbSoftDev.SOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScenarioEditor
{
    public class EventDataTemplateSelector : DataTemplateSelector
    {
        DataTemplate _scenarioObjectiveTemplate, _unitLocationTemplate, _unitTemplate, _randomTemplate, _timeTemplate;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null || !(item is BattleScriptEvent)) return null;
            FrameworkElement element = container as FrameworkElement;
            if (element == null) return null;

            // Cache templates on first use — FindResource walks the visual tree so we avoid calling it every scroll
            if (_scenarioObjectiveTemplate == null)
            {
                _scenarioObjectiveTemplate = element.FindResource("scenarioObjectiveEventTemplate") as DataTemplate;
                _unitLocationTemplate      = element.FindResource("unitLocationEventTemplate") as DataTemplate;
                _unitTemplate              = element.FindResource("unitEventTemplate") as DataTemplate;
                _randomTemplate            = element.FindResource("randomEventTemplate") as DataTemplate;
                _timeTemplate              = element.FindResource("timeEventTemplate") as DataTemplate;
            }

            if (item is ScenarioObjectiveEvent) return _scenarioObjectiveTemplate;
            if (item is UnitPositionEvent)      return _unitLocationTemplate;
            if (item is EventBase<string>)      return _unitTemplate;
            if (item is RandomEvent)            return _randomTemplate;
            if (item is TimeEvent)              return _timeTemplate;

            return null;
        }
    }

    /// <summary>
    /// these are defined in xaml
    /// </summary>
    public class CommandDataTemplateSelector : DataTemplateSelector
    {
        DataTemplate _unitArgUnit, _unitFromUnit, _unitArgDistance, _unitArgMapObjective,
                     _unitPosition, _scenarioObjective, _unitFormType, _unitFormation,
                     _randomEvent, _courier, _command;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null || !(item is Command)) return null;
            FrameworkElement element = container as FrameworkElement;
            if (element == null) return null;

            // Cache templates on first use
            if (_command == null)
            {
                _unitArgUnit         = element.FindResource("unitArgUnitCommandTemplate") as DataTemplate;
                _unitFromUnit        = element.FindResource("unitFromUnitCommandTemplate") as DataTemplate;
                _unitArgDistance     = element.FindResource("unitArgDistanceCommandTemplate") as DataTemplate;
                _unitArgMapObjective = element.FindResource("unitArgMapObjectiveCommandTemplate") as DataTemplate;
                _unitPosition        = element.FindResource("unitPositionCommandTemplate") as DataTemplate;
                _scenarioObjective   = element.FindResource("scenarioObjectiveCommandTemplate") as DataTemplate;
                _unitFormType        = element.FindResource("unitFormTypeCommandTemplate") as DataTemplate;
                _unitFormation       = element.FindResource("unitFormationCommandTemplate") as DataTemplate;
                _randomEvent         = element.FindResource("randomEventCommandTemplate") as DataTemplate;
                _courier             = element.FindResource("courierCommandTemplate") as DataTemplate;
                _command             = element.FindResource("commandTemplate") as DataTemplate;
            }

            // order is important
            if (item is UnitArgUnitCommand)        return _unitArgUnit;
            if (item is UnitFromUnitCommand)       return _unitFromUnit;
            if (item is UnitArgDistanceCommand)    return _unitArgDistance;
            if (item is UnitArgMapObjectiveCommand)return _unitArgMapObjective;
            if (item is UnitPositionCommand)       return _unitPosition;
            if (item is ScenarioObjectiveCommand)  return _scenarioObjective;
            if (item is UnitFormTypeCommand)       return _unitFormType;
            if (item is UnitFormationCommand)      return _unitFormation;
            if (item is RandomEventCommand)        return _randomEvent;
            if (item is CourierCommand)            return _courier;
            if (item is Command)                   return _command;

            return null;
        }
    }
}
