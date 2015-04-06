namespace BaseTools.UI.Navigation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "I do not want the enumeration values to be combinable.")]
    public enum NavigationMode
    {
        New = 0,
        Back = 1,
        Forward = 2,
        Reset = 4,
    }
}
