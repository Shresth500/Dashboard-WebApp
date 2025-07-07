import {
  Component,
  OnInit,
} from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../dashboard.service';
import {
  IAddproduct,
  IProductListItems,
  IProducts,
} from '../../common/Models/IAuth';
import { AddProductComponent } from '../AddProduct/add-product/add-product.component';
import { ChartComponent } from '../chart/chart.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { ChangeDetectorRef } from '@angular/core';
import { AuthService } from '../../common/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule, // ✅ This is enough
    AddProductComponent,
    ChartComponent,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  displayedColumns: string[] = ['Name', 'status', 'actions'];
  productListInfo: IProductListItems | null = null;
  productsData: IProducts[] = [];
  dataSource = new MatTableDataSource<IProducts>();
  editingRow: number | null = null;
  backupProduct: IProducts | null = null;
  statusUpdate: Map<string, number> = new Map<string, number>();

  // @ViewChild(MatPaginator) paginator!: MatPaginator;

  totalItems = 0; // Total count from the backend
  pageSize = 5; // Number of items per page
  currentPage = 1; // Current page index
  totalPages = 0;

  constructor(
    private productService: DashboardService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private authService: AuthService
  ) {}

  // Fetching the data in using OnInit  as this is the function of the
  ngOnInit() {
    this.fetchData();
    this.productService.getStatisticData().subscribe({
      next: (resp) => {
        console.log(resp);
        console.log(typeof resp);
        this.statusUpdate = new Map<string, number>(Object.entries(resp));
      },
    });
  }

  /*
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator; // ✅ Assign paginator to dataSource

    this.paginator.page.subscribe(() => {
      this.currentPage = this.paginator.pageIndex;
      this.pageSize = this.paginator.pageSize;
      this.fetchData();
    });
  }
    */

  // Pagination for next and previous button
  // To render first page
  goToFirstPage() {
    this.currentPage = 1;
    this.fetchData();
  }

  // To render the previous page
  previousPage() {
    this.currentPage -= 1;
    this.fetchData();
  }

  // To render the next page
  nextPage() {
    this.currentPage += 1;
    this.fetchData();
  }

  // To render the last page
  goToLastPage() {
    this.currentPage = this.totalPages;
    this.fetchData();
  }

  // To fetch the data from Backend
  fetchData() {
    this.productService
      .getDataByPage(this.currentPage, this.pageSize)
      .subscribe({
        next: (resp) => {
          this.productListInfo = resp; // Load data from backend
          this.dataSource.data = this.productListInfo?.products;
          this.pageSize = this.productListInfo.pageSize;
          this.totalItems = this.productListInfo.totalItems;
          this.totalPages = this.productListInfo.totalPages;
        },
      });
  }

  // Logic before editing the row
  startEditing(product: IProducts) {
    if (this.editingRow !== null) {
      alert(
        `Can't Edit the row with name ${product.name} as the row with name ${this.backupProduct?.name} is getting updated`
      );
      return;
    }
    this.editingRow = product.id;
    this.backupProduct = { ...product }; // Save original data in case of cancel
    if (!product.status) {
      product.status = 'Pending'; // Default value if status is null
    }
    this.cdr.detectChanges(); //
  }

  // Track the approved, rejected and pending status
  trackByFn(index: number, item: string) {
    return item;
  }

  // Logic for editing the product
  saveProduct(product: IProducts) {
    this.productService.updateProduct(product).subscribe({
      next: () => {
        const tempMap = new Map<string, number>(this.statusUpdate);
        if (this.backupProduct !== null) {
          tempMap.set(
            this.backupProduct.status,
            tempMap.get(this.backupProduct.status)! - 1
          );
        }
        tempMap.set(product.status, tempMap.get(product.status)! + 1);
        this.statusUpdate = tempMap;

        this.editingRow = null;
        this.backupProduct = null;
      },
    });
    this.cdr.detectChanges();
  }

  // Logic if we don't want to edit
  cancelEditing() {
    if (this.backupProduct) {
      let index = this.dataSource.data.findIndex(
        (p) => p.id === this.backupProduct!.id
      );
      this.dataSource.data[index] = this.backupProduct; // Restore original data
    }
    this.editingRow = null;
    this.backupProduct = null;
  }

  // Logic to delete the product
  deleteProduct(product: IProducts) {
    if (this.editingRow !== null) {
      alert(
        `The Edit operation for ${this.backupProduct?.name} is already happening`
      );
      return;
    }
    if (confirm(`Are you sure you want to delete ${product.name}?`)) {
      this.productService.DeleteProduct(product.id).subscribe({
        next: (resp) => {
          let val = this.statusUpdate.get(product.status);
          if (val) this.statusUpdate.set(product.status, val - 1);
          else {
            this.statusUpdate.set(product.status, 0);
          }
          console.log(resp);
          this.dataSource.data = this.dataSource.data.filter(
            (p) => p.id !== product.id
          );
        },
      });
      const tempMap = new Map<string, number>(this.statusUpdate);
      tempMap.set(product.status, tempMap.get(product.status)! - 1);
      this.statusUpdate = tempMap;
      this.cdr.detectChanges();
    }
  }

  // Adding the product
  postProduct(event: IAddproduct) {
    this.productService.postProduct(event).subscribe({
      next: (resp) => {
        const tempMap = new Map<string, number>(this.statusUpdate);
        tempMap.set(event.status, tempMap.get(event.status)! + 1);
        this.statusUpdate = tempMap;
        this.totalItems += 1;
        this.totalPages = Math.ceil(this.totalItems / this.pageSize);
        // console.log(this.totalPages);
        // this.goToLastPage();
        console.log(this.totalPages);
        this.currentPage = this.totalPages;
        console.log(this.currentPage);
        this.fetchData();
        this.cdr.detectChanges();
      },
    });
  }

  // For Logout
  logout() {
    this.authService.logout();
    this.router.navigate(['auth/login']);
  }
}
