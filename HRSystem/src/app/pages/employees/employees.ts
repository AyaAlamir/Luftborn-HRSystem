import { SlicePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { AuthService } from '../../core/auth.service';
import { Employee, EmployeeRequest, EmployeeService } from '../../core/employee.service';

@Component({selector:'app-employees',imports:[FormsModule, SlicePipe],templateUrl:'./employees.html',styleUrl:'./employees.css'})
export class Employees implements OnInit {
  employees = signal<Employee[]>([]);
  query = signal('');
  department = signal('All');
  loading = signal(false);
  error = signal('');
  modal: 'form' | 'view' | 'delete' | null = null;
  selected: Employee | null = null;
  form = this.empty();

  departmentCount = computed(() => new Set(this.employees().map(e => e.department).filter(Boolean)).size);
  totalPayroll = computed(() => this.employees().reduce((sum, employee) => sum + Number(employee.salary || 0), 0));
  departments = computed(() => [...new Set(this.employees().map(e => e.department).filter(Boolean))].sort());
  filtered = computed(() => {
    const query = this.query().trim().toLowerCase();
    return this.employees().filter(employee =>
      (!query || `${employee.firstName} ${employee.lastName} ${employee.email} ${employee.department}`.toLowerCase().includes(query)) &&
      (this.department() === 'All' || employee.department === this.department()));
  });

  constructor(public auth: AuthService, private employeeService: EmployeeService) {}

  ngOnInit(): void { this.loadEmployees(); }

  fullName(employee: Employee): string { return `${employee.firstName} ${employee.lastName}`.trim(); }
  initials(employeeOrName: Employee | string): string {
    const name = typeof employeeOrName === 'string' ? employeeOrName : this.fullName(employeeOrName);
    return name.split(' ').filter(Boolean).map(part => part[0]).slice(0, 2).join('').toUpperCase();
  }
  empty(): Employee { return { id: 0, firstName: '', lastName: '', email: '', phoneNumber: '', department: 'Engineering', salary: 0, hireDate: new Date().toISOString().slice(0, 10) }; }
  add(): void { this.selected = null; this.form = this.empty(); this.modal = 'form'; }
  edit(employee: Employee): void { this.selected = employee; this.form = { ...employee, hireDate: employee.hireDate?.slice(0, 10) }; this.modal = 'form'; }
  view(employee: Employee): void {
    this.loading.set(true);
    this.employeeService.getById(employee.id).subscribe({
      next: result => { this.loading.set(false); this.selected = result; this.modal = 'view'; },
      error: error => { this.loading.set(false); this.showError(error, 'Could not load employee details.'); }
    });
  }
  askDelete(employee: Employee): void { this.selected = employee; this.modal = 'delete'; }
  close(): void { this.modal = null; this.selected = null; }

  save(): void {
    if (!this.form.firstName.trim() || !this.form.lastName.trim() || !this.form.email.trim()) return;
    const request: EmployeeRequest = {
      firstName: this.form.firstName.trim(), lastName: this.form.lastName.trim(), email: this.form.email.trim(),
      phoneNumber: this.form.phoneNumber.trim(), department: this.form.department.trim(),
      salary: Number(this.form.salary), hireDate: this.form.hireDate
    };
    this.loading.set(true);
    const operation: Observable<unknown> = this.selected
      ? this.employeeService.update(this.selected.id, request)
      : this.employeeService.create(request);
    operation.subscribe({
      next: () => { this.loading.set(false); this.close(); this.loadEmployees(); },
      error: (error: unknown) => { this.loading.set(false); this.showError(error, 'Could not save the employee.'); }
    });
  }

  confirmDelete(): void {
    if (!this.selected) return;
    this.loading.set(true);
    this.employeeService.delete(this.selected.id).subscribe({
      next: () => { this.loading.set(false); this.close(); this.loadEmployees(); },
      error: error => { this.loading.set(false); this.showError(error, 'Could not delete the employee.'); }
    });
  }

  private loadEmployees(): void {
    this.loading.set(true);
    this.error.set('');
    this.employeeService.getAll().subscribe({
      next: employees => { this.employees.set(employees); this.loading.set(false); },
      error: error => { this.loading.set(false); this.showError(error, 'Could not load employees. Make sure the API is running.'); }
    });
  }

  private showError(error: unknown, fallback: string): void {
    const message = error instanceof HttpErrorResponse
      ? error.error?.message ?? error.error?.title ?? (typeof error.error === 'string' ? error.error : fallback)
      : fallback;
    this.error.set(message);
  }
}