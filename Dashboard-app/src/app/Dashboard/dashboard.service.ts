import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  IAddproduct,
  IProductListItems,
  IProducts,
} from '../common/Models/IAuth';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private apiUrl = `http://localhost:5121/api/Products`;

  constructor(private http: HttpClient) {}
  getDataByPage(
    pageNumber: number,
    pageSize: number
  ): Observable<IProductListItems> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    return this.http.get<IProductListItems>(`${this.apiUrl}`, {
      params,
    });
  }
  postProduct(products: IAddproduct): Observable<IProducts> {
    return this.http.post<IProducts>(`${this.apiUrl}`, products, {
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }
  updateProduct(product: IProducts): Observable<IProducts> {
    let data: Omit<IProducts, 'id'> = product;
    return this.http.put<IProducts>(`${this.apiUrl}/${product.id}`, data, {
      headers: {
        'Content-Type': 'Application/json',
      },
    });
  }
  getNumberofPage(pageSize: number): Observable<number> {
    let params = new HttpParams().set('pageSize', pageSize);
    return this.http.get<number>(`${this.apiUrl}/getpages`, { params });
  }
  DeleteProduct(id: number): Observable<IProducts> {
    return this.http.delete<IProducts>(`${this.apiUrl}/${id}`);
  }
  getStatisticData(): Observable<Map<string, number>> {
    return this.http.get<Map<string, number>>(`${this.apiUrl}/Statistics`);
  }
}
