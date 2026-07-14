import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_BASE_URL } from './api.config';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  department: string;
  salary: number;
  hireDate: string;
}

export type EmployeeRequest = Omit<Employee, 'id'>;

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/Employees`;

  // Normalize response to always return an array of employees.
  // Some backends return a plain array while others wrap the list in an object
  // (for example: { employees: [...] } or { items: [...] }). Handle both cases here.
  getAll(): Observable<Employee[]> {
    return this.http.get<any>(this.url).pipe(
      map(res => {
        if (Array.isArray(res)) return res as Employee[];
        if (!res) return [] as Employee[];
        return (res.employees || res.items || res.data || res.results || res.value || []) as Employee[];
      })
    );
  }
  // Normalize single-employee responses. Some APIs return the employee directly,
  // others wrap it ({ employee: {...} }, { data: {...} }, { value: {...} }) or even an array.
  getById(id: number): Observable<Employee | null> {
    return this.http.get<any>(`${this.url}/${id}`).pipe(
      map(res => {
        if (!res) return null;
        if (Array.isArray(res)) return (res[0] || null) as Employee | null;
        if (res.id && (res.firstName !== undefined || res.email !== undefined)) return res as Employee;
        return (res.employee || res.data || res.value || null) as Employee | null;
      })
    );
  }
  create(employee: EmployeeRequest): Observable<Employee> { return this.http.post<Employee>(this.url, employee); }
  update(id: number, employee: EmployeeRequest): Observable<void> { return this.http.put<void>(`${this.url}/${id}`, employee); }
  delete(id: number): Observable<void> { return this.http.delete<void>(`${this.url}/${id}`); }
}
