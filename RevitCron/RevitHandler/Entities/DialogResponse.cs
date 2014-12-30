using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DougKlassen.Revit.Automation
{
	/// <summary>
	/// Override settings for Revit task dialogs
	/// </summary>
	public class DialogResponse
	{
		/// <summary>
		/// The dictionary storing overrides that are in effect
		/// </summary>
		private Dictionary<String, Int32> overrides;

		/// <summary>
		/// Initialize a new instance of TaskDialogOverrides
		/// </summary>
		public DialogResponse()
		{
			overrides = new Dictionary<String, Int32>();
		}

		/// <summary>
		/// Add an override for a specified TaskDialog
		/// </summary>
		/// <param name="dialogId">The dialogId specifying the TaskDialog to override</param>
		/// <param name="resultOverride">The dialog selection to use</param>
		public void AddOverride(RevitDialog dialogType, Int32 resultOverride)
		{
			overrides.Add(StandardDialogs[dialogType], resultOverride);
		}

		/// <summary>
		/// Clear all overrides that are in effect
		/// </summary>
		public void ClearOverrides()
		{
			overrides = new Dictionary<String, Int32>();
		}

		/// <summary>
		/// Check if an override is set for a TaskDialog matching a standard Revit TaskDialog
		/// </summary>
		/// <param name="dialogType">The standard Revit TaskDialog</param>
		/// <returns>Whether the TaskDialog has an override</returns>
		public Boolean HasOverride(RevitDialog dialogType)
		{
			return HasOverride(StandardDialogs[dialogType]);
		}

		/// <summary>
		/// Check if an override is set for a TaskDialog matching the specified DialogId
		/// </summary>
		/// <param name="dialogId">The DialogId used to specify the TaskDialog</param>
		/// <returns>Whether the TaskDialog has an override</returns>
		public Boolean HasOverride(String dialogId)
		{
			if (overrides.ContainsKey(dialogId))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the override set for the specified TaskDialog
		/// </summary>
		/// <param name="dialogType">The standard Revit TaskDialog specified</param>
		/// <returns>An integer value representing the selected response to the dialog</returns>
		public Int32 GetOverride(RevitDialog dialogType)
		{
			return GetOverride(StandardDialogs[dialogType]);
		}

		/// <summary>
		/// Gets the override set for the specified TaskDialog
		/// </summary>
		/// <param name="dialogId">The Dialog Id identifying the TaskDialog</param>
		/// <returns>An integer value representing the selected response to the dialog</returns>
		public Int32 GetOverride(String dialogId)
		{
			if (HasOverride(dialogId))
			{
				return overrides[dialogId];				
			}
			else
			{
				throw new ArgumentException("No Task Dialog matching Dialog Id \"" + dialogId + "\" was found");
			}
		}

		/// <summary>
		/// Standard dialog boxes in Revit with their dialog ids
		/// </summary>
		public readonly Dictionary<RevitDialog, String> StandardDialogs =
			new Dictionary<RevitDialog, String>()
			{
					{ RevitDialog.AuditWarning, "TaskDialog_Audit_Warning" },
					{ RevitDialog.FileNameInUse, "TaskDialog_File_Name_In_Use" },
					{ RevitDialog.SaveFile, "TaskDialog_Save_File" },
					{ RevitDialog.ChangesNotSynchronized, "TaskDialog_Local_Changes_Not_Synchronized_With_Central" },
					{ RevitDialog.ChangesNotSaved, "TaskDialog_Changes_Not_Saved" },
					{ RevitDialog.CloseWithoutSaving, "TaskDialog_Close_Project_Without_Saving" },
					{ RevitDialog.LostOnImport, "TaskDialog_Elements_Lost_On_Import"},
					{ RevitDialog.UnresolvedReferences, "TaskDialog_Unresolved_References"}
			};
	}
}
