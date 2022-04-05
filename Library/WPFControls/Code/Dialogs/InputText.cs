using System;
using System.Linq;
using System.Windows;
using Mnk.Library.WpfControls.Dialogs;

namespace Mnk.Library.WpfControls.Code.Dialogs
{
	public sealed class InputText : BaseDialog
	{
		public InputText(string caption, Templates templates, Func<string, bool> validator, Func<Window> ownerGetter) :
            base(caption, templates, validator, ownerGetter)
		{
		}

		private bool BaseOp(string name, out string newName, string template)
		{
			var result = DialogsCache.ShowInputBox(template, Caption, name, x=>Validator(x) && IsAlphaNumeric(x), Owner);
			if (result.Key)
			{
				newName = result.Value;
				return true;
			}
			newName = string.Empty;
			return false;
		}

		public override bool Add(out string[] newNames)
		{
			string tmp;
			var ret = BaseOp(string.Empty, out tmp, Buttons.Add);
			newNames = string.IsNullOrWhiteSpace(tmp) ? new string[0] : new[] {tmp};
			return ret;
		}

		public override bool Edit(string name, out string newName)
		{
		    var result = DialogsCache.ShowInputBox(Buttons.Edit, Caption, name, IsAlphaNumeric, Owner);
		    if (result.Key)
		    {
		        newName = result.Value;
		        return true;
		    }
		    newName = string.Empty;
		    return false;
        }

	    private bool IsAlphaNumeric(string txt)
	    {
	        return txt.All(x => (char.ToLowerInvariant(x) >= 'a' && char.ToLowerInvariant(x) <= 'z') ||
	                            (x >= '0' && x <= '9') || 
                                (x == '@') );
	    }
	}
}
