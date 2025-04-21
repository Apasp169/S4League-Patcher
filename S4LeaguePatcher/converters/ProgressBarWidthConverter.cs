using System.Globalization;
using System.Windows.Data;

namespace S4LeaguePatcher.converters;

/// <summary>
///     Converts progress values into appropriate width measurements for WPF progress bars.
/// </summary>
/// <remarks>
///     This converter is used in XAML to calculate the visual width of a progress bar based on
///     the current value, minimum, maximum, and available track width. It implements
///     <see cref="IMultiValueConverter" /> to handle multiple input values in the conversion process.
///     Typical usage in XAML:
///     <code>
/// &lt;ProgressBar.Width&gt;
///     &lt;MultiBinding Converter="{StaticResource ProgressBarWidthConverter}"&gt;
///         &lt;Binding Path="Value"/&gt;
///         &lt;Binding Path="Minimum"/&gt;
///         &lt;Binding Path="Maximum"/&gt;
///         &lt;Binding Path="ActualWidth" ElementName="TrackElement"/&gt;
///     &lt;/MultiBinding&gt;
/// &lt;/ProgressBar.Width&gt;
/// </code>
/// </remarks>
public class ProgressBarWidthConverter : IMultiValueConverter
{
    /// <summary>
    ///     Converts an array of progress-related values into a width measurement.
    /// </summary>
    /// <param name="values">
    ///     An array that must contain exactly four double values in the following order:
    ///     [0]: Current value of the progress
    ///     [1]: Minimum value of the progress range
    ///     [2]: Maximum value of the progress range
    ///     [3]: Available track width in pixels
    /// </param>
    /// <param name="targetType">The type of the binding target property (should be double).</param>
    /// <param name="parameter">Not used in this implementation.</param>
    /// <param name="culture">The culture information to use in the conversion.</param>
    /// <returns>
    ///     A double representing the calculated width of the progress indicator in pixels,
    ///     or 0.0 if the inputs are invalid or the range is zero.
    /// </returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [double value, double minimum, double maximum, double trackWidth])
            return 0.0;

        if (maximum - minimum == 0)
            return 0.0;

        var percent = (value - minimum) / (maximum - minimum);
        return percent * trackWidth;
    }

    /// <summary>
    ///     Not implemented as this converter is designed for one-way binding only.
    /// </summary>
    /// <exception cref="NotImplementedException">
    ///     Always thrown as this method is not implemented.
    /// </exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}