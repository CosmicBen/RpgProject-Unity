namespace Rpg.Control
{
    public interface IRaycastable
    {
        CursorType GetCursorType();

        bool HandleRaycast(PlayerController callingController);
    }
}