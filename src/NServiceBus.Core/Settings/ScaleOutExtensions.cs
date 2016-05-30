namespace NServiceBus
{
    using Settings;

    /// <summary>
    /// Provides a fluent api to allow the configuration of <see cref="ScaleOutSettings" />.
    /// </summary>
    public static class ScaleOutExtensions
    {
        /// <summary>
        /// Allows the user to control how the current endpoint behaves when scaled out.
        /// </summary>
        /// <param name="config">The <see cref="EndpointConfiguration" /> instance to apply the settings to.</param>
        public static ScaleOutSettings ScaleOut(this EndpointConfiguration config)
        {
            Guard.AgainstNull(nameof(config), config);
            return new ScaleOutSettings(config);
        }
    }
}