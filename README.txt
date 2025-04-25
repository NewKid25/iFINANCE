To run:


1. In MS Sql Server Manager, restore the "Group7_iFINANCEDB.bak" file so that you have our database structure and sample data
2. Open the "Group7_iFINANCEAPP.sln" file in Visual Studio
3. Find the "Web.config" file, and within it edit the "data-source" property of the "connectionStrings" element to match your local database server name
4. Run the solution from Visual Studio