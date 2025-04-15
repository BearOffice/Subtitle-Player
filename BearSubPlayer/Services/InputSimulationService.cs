using WindowsInput.Native;
using WindowsInput;

namespace BearSubPlayer.Services;

public class InputSimulationService
{
    private readonly InputSimulator _sim;
    
    public InputSimulationService(InputSimulator sim)
    {
        _sim = sim;
    }

    public void SpaceKey(int delay)
    {
        Task.Delay(delay).Wait();
        _sim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
    }

    public void MouseLeftClick(int delay)
    {
        Task.Delay(delay).Wait();
        _sim.Mouse.LeftButtonClick();
    }
}