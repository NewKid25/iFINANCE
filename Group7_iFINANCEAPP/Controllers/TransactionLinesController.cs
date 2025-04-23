using Group7_iFINANCEAPP.Models;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using System.Web.Mvc;

@model Group7_iFINANCEAPP.Models.TransactionLine

@{
    ViewBag.Title = "Edit";
}

< div class= "container my-5" >
    < div class= "card shadow-sm rounded-4 p-5" >
        < h2 class= "text-center mb-4" > Edit Transaction Line</h2>

        @using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
            @Html.ValidationSummary(true, "", new { @class = "text-danger text-center mb-3" })

            < div class= "mb-3" >
                @Html.LabelFor(model => model.creditedAmount, "Credit Value", htmlAttributes: new { @class = "form-label" })
                @Html.EditorFor(model => model.creditedAmount, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.creditedAmount, "", new { @class = "text-danger" })
            </ div >

            < div class= "mb-3" >
                @Html.LabelFor(model => model.MasterAccountID, "Credit Account", htmlAttributes: new { @class = "form-label" })
                @Html.DropDownList("MasterAccountID", null, htmlAttributes: new { @class = "form-control" })
                @Html.ValidationMessageFor(model => model.MasterAccountID, "", new { @class = "text-danger" })
            </ div >

            < div class= "mb-3" >
                @Html.LabelFor(model => model.debitedAmount, "Debit Value", htmlAttributes: new { @class = "form-label" })
                @Html.EditorFor(model => model.debitedAmount, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.debitedAmount, "", new { @class = "text-danger" })
            </ div >

            < div class= "mb-3" >
                @Html.LabelFor(model => model.MasterAccountID2, "Debit Account", htmlAttributes: new { @class = "form-label" })
                @Html.DropDownList("MasterAccountID2", null, htmlAttributes: new { @class = "form-control" })
                @Html.ValidationMessageFor(model => model.MasterAccountID2, "", new { @class = "text-danger" })
            </ div >

            < div class= "mb-3" >
                @Html.LabelFor(model => model.comment, "Comment", htmlAttributes: new { @class = "form-label" })
                @Html.EditorFor(model => model.comment, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.comment, "", new { @class = "text-danger" })
            </ div >

            < div class= "mb-4" >
                @Html.Label("TransactionID", "Transaction ID (not editable)", htmlAttributes: new { @class = "form-label" })
                < p class= "form-control-plaintext" > @Model.TransactionID </ p >
                @Html.HiddenFor(model => model.TransactionID)
                @Html.ValidationMessageFor(model => model.TransactionID, "", new { @class = "text-danger" })
            </ div >

            < div class= "text-center" >
                < input type = "submit" value = "Save" class= "btn btn-primary btn-lg me-3" />
                @Html.ActionLink("Back to List", "Index", "Transactions", null, new { @class = "btn btn-secondary btn-lg" })
            </ div >
        }
    </ div >
</ div >