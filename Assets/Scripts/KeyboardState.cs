namespace Gunplay.KeyboardState
{
    public struct KeyboardState {
        public bool up;
        public bool down;
        public bool left;
        public bool right;
        public bool attack;

        public KeyboardState(bool up, bool down, bool left, bool right, bool attack)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
            this.attack = attack;
        }
    }
}
