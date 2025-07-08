## PRODUCT CATALOG

### Technologies Used

1. To Create Api’s - Csharp and .net web api
2. Database - Sqlite
3. Front-end – Angular
4. To make forms – ng-bootstrap
5. To make doughnut – Chart.js and ChartDataLabel.js modules
6. To make Product List and Table - Material UI {MatTableModule, MatTableDataSourceModule, MatCardModule,MatFormFieldModule,MatSelectModule, AddProductComponent, ChartComponent,FormsModule,ReactiveFormsModule,MatInputModule}
7. For extra styling – custom css

### Welcome Page

1. Login Page

2. Registration Page


### Login Page Validation

1. Email Validation from Backend
2. Email Validation
3. Password Validation

### REGISTRATION VALIDATION

1. Password Validations
2. If Password and Confirm Password doesn’t match
3. Email validation

### Authentication Work Flow

1. First if user is not register, it can regist after succ registration is successful then it would redirect to Login Page, where it will check the login credentials, if the credentials are correct then it will redirect to dashboard page

Registration Page

Login Page

Dashboard Page

1. In the Dashboard page we can add the product
2. We can see the Status Distribution using dougnut chart
3. See the product List
4. Edit a Product
5. Delete a Product
6. After adding/ deleting / updating the fields the pie chart would be automatically updated without reloading the page

### Adding Product

1. If we click on Add, then the product is being added which is getting displayed on the last page inside the Product List and it would update on pie chart
2. As you can see in the below image that the percentage of pending has increased from 42.86% to 46.67%
3. The last page has been rendered as according to the pagination the item is added always in the last page is added

### Editing Product

1. When you click on Edit Option then the Name and status field becomes editable
2. Then you can change the name of the Product Name and can change status from the drop down.
3. After Clicking on save the changes will be reflected and it would be reflected on the doughnut chart as well.
4. Remember you can’t edit more fields at the same time
5. If you are editing one field you can’t delete other fields until and unless you make changes or cancel the changes.


6. After editing if you try to edit any other row, it will give an alert stating you can’t edit more than one row at the same time.


7. Same is the case for Delete, until an unless you haven’t save the edited row or cancelled the editing it will not allow you to perform any operation on any other row.

