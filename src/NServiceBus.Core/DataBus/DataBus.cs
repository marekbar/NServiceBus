namespace NServiceBus.Features
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus.DataBus;
    using Settings;

    /// <summary>
    /// Used to configure the databus.
    /// </summary>
    public class DataBus : Feature
    {
        internal DataBus()
        {
            Defaults(s => s.EnableFeatureByDefault(GetSelectedFeatureForDataBus(s)));
        }

        static Type GetSelectedFeatureForDataBus(SettingsHolder settings)
        {
            DataBusDefinition dataBusDefinition;

            if (!settings.TryGet("SelectedDataBus", out dataBusDefinition))
            {
                dataBusDefinition = new FileShareDataBus();
            }

            return dataBusDefinition.ProvidedByFeature();
        }

        /// <summary>
        /// Called when the features is activated.
        /// </summary>
        protected internal override void Setup(FeatureConfigurationContext context)
        {
            if (!context.Container.HasComponent<IDataBusSerializer>())
            {
                context.Container.ConfigureComponent<DefaultDataBusSerializer>(DependencyLifecycle.SingleInstance);
            }

            context.RegisterStartupTask(b => b.Build<IDataBusInitializer>());
            context.Container.ConfigureComponent<IDataBusInitializer>(DependencyLifecycle.SingleInstance);

            var conventions = context.Settings.Get<Conventions>();
            context.Pipeline.Register(new DataBusReceiveBehavior.Registration(conventions));
            context.Pipeline.Register(new DataBusSendBehavior.Registration(conventions));
        }

        class IDataBusInitializer : FeatureStartupTask
        {
            public IDataBus DataBus { get; set; }

            protected override Task OnStart(IMessageSession session)
            {
                return DataBus.Start();
            }

            protected override Task OnStop(IMessageSession session)
            {
                return TaskEx.CompletedTask;
            }
        }
    }
}