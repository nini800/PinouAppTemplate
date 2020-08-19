using System.Collections.Generic;

namespace Pinou.InputSystem
{
    public enum FocusMode
    {
        Exclusive,
        Additive
    }
	public class PinouPlayer
	{
        #region Constructors
        public void Construct(int playerID, string playerName, int controllerId)
		{
            _playerID = playerID;
            _playerName = playerName;
            _controllerId = controllerId;
		}
        #endregion

        #region Attributes, Accessors & Mutators
        private int _playerID;
        private int _controllerId;
        private string _playerName;
        public int PlayerID => _playerID;
        public int ControllerId => _controllerId;
        public string PlayerName => _playerName;

        public ControllerType ControllerType => PinouApp.Input.GetControllerType(_controllerId);

        private List<PinouInputReceiver> _focuses = new List<PinouInputReceiver>();
        public PinouInputReceiver[] Focuses => _focuses.ToArray();

        public bool IsFocused(PinouInputReceiver ir)
		{
			return _focuses.Contains(ir);
		}
        #endregion

        #region Utilities
        public void Focus(PinouInputReceiver ir, FocusMode mode = FocusMode.Additive)
        {
            if (mode == FocusMode.Exclusive)
            {
                _focuses.Clear();
            }

            if (_focuses.Contains(ir) == false)
            {
                _focuses.Add(ir);
                ir.ReceiveFocus(this);
            }
        }
        public void RemoveFocus(PinouInputReceiver ir)
        {
            if (_focuses.Contains(ir) == true)
            {
                _focuses.Remove(ir);
                ir.LooseFocus(this);
            }
        }
        public void RemoveAllFocuses()
        {
            for (int i = _focuses.Count - 1; i >= 0; i--)
            {
                PinouInputReceiver ir = _focuses[i];
                _focuses.RemoveAt(i);
                ir.LooseFocus(this);
            }
        }
        public void OverrideControllerID(int newControllerId)
        {
            _controllerId = newControllerId;
        }

        public void Destroy()
        {
            RemoveAllFocuses();
        }
        #endregion
    }
}