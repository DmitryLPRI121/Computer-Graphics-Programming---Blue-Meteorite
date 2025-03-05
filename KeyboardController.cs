using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class KeyboardController
    {
        private Camera camera;
        private HashSet<Keys> pressedKeys;
        private float deltaTime;

        public KeyboardController(Camera camera, HashSet<Keys> pressedKeys, float deltaTime)
        {
            this.camera = camera;
            this.pressedKeys = pressedKeys;
            this.deltaTime = deltaTime;
        }

        public void ProcessKeyboardInput()
        {
            Vector3 right = Vector3.Cross(camera.Front, camera.Up).Normalized();

            if (pressedKeys.Contains(Keys.W)) camera.Move(camera.Front, deltaTime);
            if (pressedKeys.Contains(Keys.S)) camera.Move(-camera.Front, deltaTime);
            if (pressedKeys.Contains(Keys.A)) camera.Move(-right, deltaTime);
            if (pressedKeys.Contains(Keys.D)) camera.Move(right, deltaTime);

            if (pressedKeys.Contains(Keys.Space)) camera.Move(camera.Up, deltaTime);
            if (pressedKeys.Contains(Keys.ControlKey)) camera.Move(-camera.Up, deltaTime);
        }
    }
}