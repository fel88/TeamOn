namespace TeamOn.Controls
{
    public interface IFocusContainer
    {
        bool Focused { get; set; }

        void LostFocus();
    }
}
