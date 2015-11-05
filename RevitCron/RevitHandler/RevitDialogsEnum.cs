using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DougKlassen.Revit.Automation
{
	/// <summary>
	/// Standard Revit task dialogs
	/// </summary>
	public enum RevitDialog
	{
		/// <summary>
		/// Checking Audit
		/// ==================================
		/// HelpId: -1
		/// DialogId: TaskDialog_Audit_Warning
		/// Message: This operation can take a long time. Recommended uses includes periodic maintenance of large files and preparation for upgrading to a new release. Do you want to continue?
		///		6:	Yes
		///		7:	No
		/// </summary>
		AuditWarning,
		/// <summary>
		/// If the project is saved but not synchronized
		/// ==================================
		/// HelpId: -1
		/// DialogId: TaskDialog_Local_Changes_Not_Synchronized_With_Central
		/// Message: You have made changes to this file that have not been synchronized with the central model. What do you want to do?
		/// 	1001:	Synchronize with central
		/// 	1002:	Close the local file
		/// 	2:		Cancel
		/// </summary>
		ChangesNotSynchronized,
		/// <summary>
		/// If the project isn't saved or synchronized:
		/// ==================================
		/// HelpId: -1
		/// DialogId: TaskDialog_Changes_Not_Saved
		/// Message: You have made changes to this file that have not been saved. What do you want to do?
		/// 	1001:	Synchronize with central
		/// 	1002:	Save locally
		/// 	1003:	Do not save the project
		/// 	2:		Cancel
		/// </summary>
		ChangesNotSaved,
		/// <summary>
		/// If an un-synchronized workshared project is being closed without saving
		/// =======================
		/// HelpId: -1
		/// DialogId: TaskDialog_Close_Project_Without_Saving
		/// Message: What do you want to do with the elements or worksets you have checked out?
		/// 	1001:	Relinquish all elements and worksets
		/// 	1002:	Keep ownership of all elements and worksets
		/// 	2:		Cancel
		/// </summary>
		CloseWithoutSaving,
		/// <summary>
		/// When the family template file location specified can't be located
		/// </summary>
		/// <remarks>
		/// DialogId: TaskDialog_Default_Family_Template_File_Invalid
		/// Message: The path you have specified for the default family template file is invalid.
		///		8:	Close
		/// </remarks>
		DefaultFamilyTemplateInvalid,
		/// <summary>
		/// When detaching a model from central
		/// </summary>
		/// <remarks>
		/// DialogId: TaskDialog_Detach_Model_From_Central
		/// Message: Detaching this model will create an independent model. You will be unable to synchronize your changes with the original central model. What do you want to do?
		///		1001: Detach and preserve worksets
		///		1002: Detach and discard worksets
		///		2:		Cancel
		/// </remarks>
		DetachModel,
		/// <summary>
		/// Creating a new local when one already exists for the project
		/// ==================================
		/// HelpId: -1
		/// DialogId: TaskDialog_File_Name_In_Use
		/// Message: You are trying to create a new local file *** but a file with this name already exists. What do you want to do?
		///		1001:	Overwrite existing file
		///		1002:	Append timestamp to existing filename
		///		2:		Cancel
		/// </summary>
		FileNameInUse,
		/// <summary>
		/// When project is opened and ActiveX elements were lost on import
		/// </summary>
		/// <remarks>
		/// DialogId: TaskDialog_Lost_On_Import
		/// Message: Some elements were lost during import. ActiveX(r) and some proprietary components cannot be imported
		/// 1:	Ok
		/// </remarks>
		LostOnImport,
		/// <summary>
		/// If the project is not workshared and has unsaved changes
		/// ==================================
		/// HelpId: -1
		/// DialogId: TaskDialog_Save_File
		/// Message: Do you want to save changes to ***.r**?
		/// 	6:	Yes
		/// 	7:	No
		/// 	2:	Cancel
		/// </summary>
		SaveFile,
		/// <summary>
		/// When there are unresolved references during project open
		/// </summary>
		/// <remarks>
		/// DialogId: TaskDialog_Unresolved_References
		/// Message: Revit could not find or read {number} referenced files. What do you want to do?
		/// 1001: Open Manage Links to correct the problem
		/// 1002: Ignore and continue opening the project
		/// </remarks>
		UnresolvedReferences
	}
}
