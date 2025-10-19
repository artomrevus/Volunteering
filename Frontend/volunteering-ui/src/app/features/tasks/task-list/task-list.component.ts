import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { TaskDto, TaskFilterDto, TaskSortingDto, PaginationDto } from '../../../core/models/interfaces';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="task-list-container">
      <div class="header">
        <h1>Tasks</h1>
        @if (user()?.role === 'MILITARY') {
          <a routerLink="/tasks/create" class="btn btn-primary">Create Task</a>
        }
      </div>

      <div class="filters">
        <div class="filter-group">
          <label>Status</label>
          <select [(ngModel)]="filter.status" (change)="applyFilters()">
            <option value="">All</option>
            <option value="CREATED">Created</option>
            <option value="IN_PROGRESS">In Progress</option>
            <option value="BLOCKED">Blocked</option>
            <option value="DELIVERING">Delivering</option>
            <option value="FINISHED">Finished</option>
            <option value="CONFIRMED">Confirmed</option>
          </select>
        </div>

        <div class="filter-group">
          <label>Priority</label>
          <select [(ngModel)]="filter.priority" (change)="applyFilters()">
            <option value="">All</option>
            <option value="LOW">Low</option>
            <option value="AVERAGE">Average</option>
            <option value="HIGH">High</option>
          </select>
        </div>

        <div class="filter-group">
          <label>Sort By</label>
          <select [(ngModel)]="sorting.sortBy" (change)="applyFilters()">
            <option value="CreatedAt">Created Date</option>
            <option value="Priority">Priority</option>
          </select>
        </div>

        <div class="filter-group">
          <label>Order</label>
          <select [(ngModel)]="sorting.isDescending" (change)="applyFilters()">
            <option [value]="false">Ascending</option>
            <option [value]="true">Descending</option>
          </select>
        </div>
      </div>

      <div class="tasks-grid">
        @for (task of tasks; track task.id) {
          <div class="task-card">
            <div class="task-header">
              <h3>{{ task.title }}</h3>
              <span class="priority-badge" [class]="'priority-' + task.priority.toLowerCase()">
                {{ task.priority }}
              </span>
            </div>
            
            <p class="task-description">{{ task.description }}</p>
            
            <div class="task-meta">
              <span class="status-badge" [class]="'status-' + task.status.toLowerCase()">
                {{ task.status }}
              </span>
              <span class="date">{{ task.createdAt | date:'short' }}</span>
            </div>

            <div class="task-actions">
              <a [routerLink]="['/tasks', task.id]" class="btn btn-sm btn-secondary">View</a>
              
              @if (canEditTask(task)) {
                <a [routerLink]="['/tasks/edit', task.id]" class="btn btn-sm btn-primary">Edit</a>
              }
              
              @if (canStartTask(task)) {
                <button (click)="startTask(task.id)" class="btn btn-sm btn-success">Start</button>
              }
              
              @if (canUpdateStatus(task)) {
                <button (click)="showStatusUpdate(task)" class="btn btn-sm btn-info">Update Status</button>
              }
              
              @if (canConfirmTask(task)) {
                <button (click)="confirmTask(task.id)" class="btn btn-sm btn-success">Confirm</button>
              }
            </div>
          </div>
        }
      </div>

      @if (tasks.length === 0) {
        <div class="no-tasks">
          <p>No tasks found</p>
        </div>
      }

      <div class="pagination">
        <button 
          (click)="previousPage()" 
          [disabled]="pagination.pageNumber === 1"
          class="btn btn-secondary"
        >
          Previous
        </button>
        <span>Page {{ pagination.pageNumber }}</span>
        <button 
          (click)="nextPage()" 
          [disabled]="tasks.length < pagination.pageSize"
          class="btn btn-secondary"
        >
          Next
        </button>
      </div>
    </div>
  `,
  styles: [`
    .task-list-container {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }

    .filters {
      display: flex;
      gap: 20px;
      margin-bottom: 30px;
      flex-wrap: wrap;
    }

    .filter-group {
      display: flex;
      flex-direction: column;
      gap: 5px;
    }

    .filter-group label {
      font-size: 14px;
      color: #666;
      font-weight: 500;
    }

    .filter-group select {
      padding: 8px 12px;
      border: 1px solid #ddd;
      border-radius: 6px;
      background: white;
    }

    .tasks-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 20px;
      margin-bottom: 30px;
    }

    .task-card {
      background: white;
      border-radius: 12px;
      padding: 20px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      transition: transform 0.2s;
    }

    .task-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }

    .task-header {
      display: flex;
      justify-content: space-between;
      align-items: start;
      margin-bottom: 10px;
    }

    .task-header h3 {
      flex: 1;
      margin: 0;
      color: #333;
    }

    .task-description {
      color: #666;
      margin-bottom: 15px;
      line-height: 1.5;
    }

    .task-meta {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 15px;
    }

    .date {
      color: #999;
      font-size: 14px;
    }

    .task-actions {
      display: flex;
      gap: 10px;
      flex-wrap: wrap;
    }

    .btn {
      padding: 10px 20px;
      border: none;
      border-radius: 6px;
      font-weight: 600;
      cursor: pointer;
      text-decoration: none;
      display: inline-block;
      transition: all 0.3s;
    }

    .btn-sm {
      padding: 6px 12px;
      font-size: 14px;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover {
      background: #5a67d8;
    }

    .btn-secondary {
      background: #e2e8f0;
      color: #333;
    }

    .btn-secondary:hover {
      background: #cbd5e0;
    }

    .btn-success {
      background: #48bb78;
      color: white;
    }

    .btn-success:hover {
      background: #38a169;
    }

    .btn-info {
      background: #4299e1;
      color: white;
    }

    .btn-info:hover {
      background: #3182ce;
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .priority-badge, .status-badge {
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 600;
      text-transform: uppercase;
    }

    .priority-low {
      background: #d4edda;
      color: #155724;
    }

    .priority-average {
      background: #fff3cd;
      color: #856404;
    }

    .priority-high {
      background: #f8d7da;
      color: #721c24;
    }

    .status-created {
      background: #e2e8f0;
      color: #333;
    }

    .status-in_progress {
      background: #bee3f8;
      color: #2c5282;
    }

    .status-blocked {
      background: #fed7d7;
      color: #742a2a;
    }

    .status-delivering {
      background: #fbb6ce;
      color: #702459;
    }

    .status-finished {
      background: #c6f6d5;
      color: #22543d;
    }

    .status-confirmed {
      background: #9ae6b4;
      color: #22543d;
    }

    .no-tasks {
      text-align: center;
      padding: 60px;
      color: #999;
    }

    .pagination {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 20px;
    }
  `]
})
export class TaskListComponent implements OnInit {
  tasks: TaskDto[] = [];
  filter: TaskFilterDto = {};
  sorting: TaskSortingDto = {
    sortBy: 'CreatedAt',
    isDescending: true
  };
  pagination: PaginationDto = {
    pageNumber: 1,
    pageSize: 10
  };

  constructor(
    private taskService: TaskService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  get user() {
    return this.authService.currentUser;
  }

  loadTasks(): void {
    this.taskService.filterTasks(this.filter, this.sorting, this.pagination).subscribe({
      next: (tasks) => {
        this.tasks = tasks.items;
        console.log('Loaded tasks:', this.tasks);
      },
      error: (error) => {
        console.error('Failed to load tasks', error);
      }
    });
  }

  applyFilters(): void {
    this.pagination.pageNumber = 1;
    this.loadTasks();
  }

  nextPage(): void {
    this.pagination.pageNumber++;
    this.loadTasks();
  }

  previousPage(): void {
    if (this.pagination.pageNumber > 1) {
      this.pagination.pageNumber--;
      this.loadTasks();
    }
  }

  canEditTask(task: TaskDto): boolean {
    const user = this.user();
    return user?.role === 'MILITARY' && task.militaryId === user.id && task.status === 'CREATED';
  }

  canStartTask(task: TaskDto): boolean {
    const user = this.user();
    return user?.role === 'VOLUNTEER' && task.status === 'CREATED';
  }

  canUpdateStatus(task: TaskDto): boolean {
    const user = this.user();
    return user?.role === 'VOLUNTEER' && 
           task.volunteerId === user.id && 
           ['IN_PROGRESS', 'BLOCKED', 'DELIVERING'].includes(task.status);
  }

  canConfirmTask(task: TaskDto): boolean {
    const user = this.user();
    return user?.role === 'MILITARY' && 
           task.militaryId === user.id && 
           task.status === 'FINISHED';
  }

  startTask(taskId: string): void {
    this.taskService.startTask({ taskId }).subscribe({
      next: () => {
        this.loadTasks();
      },
      error: (error) => {
        console.error('Failed to start task', error);
      }
    });
  }

  confirmTask(taskId: string): void {
    this.taskService.confirmTask({ taskId }).subscribe({
      next: () => {
        this.loadTasks();
      },
      error: (error) => {
        console.error('Failed to confirm task', error);
      }
    });
  }

  showStatusUpdate(task: TaskDto): void {
    // Implement status update modal/dialog
    const newStatus = prompt('Enter new status (IN_PROGRESS, BLOCKED, DELIVERING, FINISHED):');
    if (newStatus && ['IN_PROGRESS', 'BLOCKED', 'DELIVERING', 'FINISHED'].includes(newStatus)) {
      this.taskService.updateTaskStatus({ 
        taskId: task.id, 
        status: newStatus as any 
      }).subscribe({
        next: () => {
          this.loadTasks();
        },
        error: (error) => {
          console.error('Failed to update task status', error);
        }
      });
    }
  }
}