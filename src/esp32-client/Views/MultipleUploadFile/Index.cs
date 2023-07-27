﻿// @model MultipleUploadFileModel
// @{
//     ViewData["Title"] = "Upload Files";
// }

// @using (Html.BeginForm("Index", "MultipleUploadFile", FormMethod.Post, new { @enctype = "multipart/form-data" }))
// {
//     <div class="row" style="border: groove; margin: 1px;">
//         @{
//             await Html.RenderPartialAsync("ListFile", Model);
//             await Html.RenderPartialAsync("ListServer", Model);
//         }
//     </div>

//     <div class="multi-upload-option">
//         @Html.CheckBoxFor(s=> s.ReplaceIfExist)
//         <span>Replace if exists</span>
//     </div>

//     <div class="submit">
//         <input type="submit" value="Upload" />
//     </div>
// }

// @section Scripts{
//     <script>

//         var toggles = document.querySelectorAll('.toggle-folder');

//         toggles.forEach(function (toggle) {
//             toggle.addEventListener('click', function () {
//                 var parentLi = toggle.parentNode;
//                 var nestedUl = parentLi.querySelector('ul');

//                 if (nestedUl.style.display === 'none') {
//                     nestedUl.style.display = 'block';
//                     toggle.classList.remove("fa-folder");
//                     toggle.classList.add("fa-folder-open");
//                 } else {
//                     nestedUl.style.display = 'none';
//                     toggle.classList.remove("fa-folder-open");
//                     toggle.classList.add("fa-folder");
//                 }
//             });
//         });

//     </script>
//     <style>
//         ul {
//             list-style-type: none;
//         }

//         li {
//             list-style-type: none;
//         }

//         .multi-upload-option {
//             text-align: center;
//             margin: 10px;
//         }

//         .submit {
//             text-align: center;
//             margin-bottom: 20px;
//         }

//         span {
//             font-weight: 450;
//         }
//     </style>
// } 