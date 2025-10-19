import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  TaskDto,
  CreateTaskDto,
  UpdateTaskDto,
  ConfirmTaskDto,
  UpdateTaskStatusDto,
  TaskFilterDto,
  TaskSortingDto,
  PaginationDto
} from '../models/interfaces';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = environment.API_BASE_URL;

  constructor(private http: HttpClient) {}

  createTask(dto: CreateTaskDto): Observable<TaskDto> {
    return this.http.post<TaskDto>(`${this.apiUrl}/tasks`, dto);
  }

  updateTask(dto: UpdateTaskDto): Observable<TaskDto> {
    return this.http.put<TaskDto>(`${this.apiUrl}/tasks`, dto);
  }

  confirmTask(dto: ConfirmTaskDto): Observable<TaskDto> {
    return this.http.post<TaskDto>(`${this.apiUrl}/tasks/confirm`, dto);
  }

  startTask(dto: ConfirmTaskDto): Observable<TaskDto> {
    return this.http.post<TaskDto>(`${this.apiUrl}/tasks/start`, dto);
  }

  updateTaskStatus(dto: UpdateTaskStatusDto): Observable<TaskDto> {
    return this.http.post<TaskDto>(`${this.apiUrl}/tasks/status`, dto);
  }

  getTask(id: string): Observable<TaskDto> {
    return this.http.get<TaskDto>(`${this.apiUrl}/tasks/${id}`);
  }

  filterTasks(
    filter?: TaskFilterDto,
    sorting?: TaskSortingDto,
    pagination?: PaginationDto
  ): Observable<{ items: TaskDto[] }> {
    let params = new HttpParams();
    
    if (filter) {
      Object.keys(filter).forEach(key => {
        const value = (filter as any)[key];
        if (value !== null && value !== undefined) {
          params = params.set(key, value.toString());
        }
      });
    }
    
    if (sorting) {
      params = params.set('sortBy', sorting.sortBy);
      params = params.set('isDescending', sorting.isDescending.toString());
    }
    
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<{ items: TaskDto[] }>(`${this.apiUrl}/tasks/filter`, { params });
  }
}