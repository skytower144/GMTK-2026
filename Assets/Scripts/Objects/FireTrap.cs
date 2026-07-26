
public class FireTrap : Trap
{
    protected override void ActivateTrap()
    {
        gameControl.PlayerControl.Knockback();
    }
}
