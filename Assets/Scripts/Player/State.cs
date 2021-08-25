namespace Deb.Player.States
{
    public abstract class State
    {
        public abstract void OnEnterState(Player player);
        public abstract void Update(Player player);
        public abstract void FixedUpdate(Player player);
    }
}