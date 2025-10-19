import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { TaskService } from '../../core/services/task.service';
import { TaskDto } from '../../core/models/interfaces';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="dashboard">
      <div class="header">
        <h1>Dashboard</h1>
        <div class="user-info">
          <span>Welcome, {{ user()?.username }}</span>
          <span class="role-badge">{{ user()?.role }}</span>
        </div>
      </div>

      <div class="stats-grid">
        <div class="stat-card">
          <h3>Total Tasks</h3>
          <p class="stat-number">{{ tasks.length }}</p>
        </div>
        <div class="stat-card">
          <h3>Active Tasks</h3>
          <p class="stat-number">{{ getActiveTasksCount() }}</p>
        </div>
        <div class="stat-card">
          <h3>Completed Tasks</h3>
          <p class="stat-number">{{ getCompletedTasksCount() }}</p>
        </div>
        <div class="stat-card">
          <h3>High Priority</h3>
          <p class="stat-number">{{ getHighPriorityCount() }}</p>
        </div>
      </div>

      <div class="actions">
        @if (user()?.role === 'MILITARY') {
          <a routerLink="/tasks/create" class="btn btn-primary">Create New Task</a>
        }
        <a routerLink="/tasks" class="btn btn-secondary">View All Tasks</a>
      </div>

      <div class="recent-tasks">
        <h2>Recent Tasks</h2>
        <div class="task-list">
          @for (task of recentTasks; track task.id) {
            <div class="task-card">
              <div class="task-header">
                <h3>{{ task.title }}</h3>
                <span class="priority-badge" [class]="'priority-' + task.priority.toLowerCase()">
                  {{ task.priority }}
                </span>
              </div>
              <p class="task-description">{{ task.description }}</p>
              <div class="task-footer">
                <span class="status-badge" [class]="'status-' + task.status.toLowerCase()">
                  {{ task.status }}
                </span>
                <a [routerLink]="['/tasks', task.id]" class="view-link">View Details →</a>
              </div>
            </div>
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard {
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

    .user-info {
      display: flex;
      gap: 15px;
      align-items: center;
    }

    .role-badge {
      background: #667eea;
      color: white;
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 14px;
      font-weight: 600;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 20px;
      margin-bottom: 30px;
    }

    .stat-card {
      background: white;
      padding: 20px;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .stat-card h3 {
      color: #666;
      font-size: 14px;
      margin-bottom: 10px;
    }

    .stat-number {
      font-size: 32px;
      font-weight: bold;
      color: #333;
    }

    .actions {
      display: flex;
      gap: 15px;
      margin-bottom: 30px;
    }

    .btn {
      padding: 12px 24px;
      border-radius: 8px;
      text-decoration: none;
      font-weight: 600;
      transition: all 0.3s;
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

    .recent-tasks {
      margin-top: 40px;
    }

    .recent-tasks h2 {
      margin-bottom: 20px;
    }

    .task-list {
      display: grid;
      gap: 20px;
    }

    .task-card {
      background: white;
      padding: 20px;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .task-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 10px;
    }

    .task-description {
      color: #666;
      margin-bottom: 15px;
    }

    .task-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .priority-badge {
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 600;
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

    .status-badge {
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 600;
    }

    .status-created {
      background: #e2e8f0;
      color: #333;
    }

    .status-in_progress {
      background: #bee3f8;
      color: #2c5282;
    }

    .status-finished {
      background: #c6f6d5;
      color: #22543d;
    }

    .view-link {
      color: #667eea;
      text-decoration: none;
      font-weight: 600;
    }

    .view-link:hover {
      text-decoration: underline;
    }
  `]
})
export class DashboardComponent implements OnInit {
  tasks: TaskDto[] = [];
  recentTasks: TaskDto[] = [];

  constructor(
    public authService: AuthService,
    private taskService: TaskService
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  get user() {
    return this.authService.currentUser;
  }

  loadTasks(): void {
    const user = this.user();
    if (!user) return;

    const filter = user.role === 'MILITARY' 
      ? { militaryId: user.id }
      : { volunteerId: user.id };

    this.taskService.filterTasks(filter).subscribe({
      next: (tasks) => {
        this.tasks = tasks.items;
        this.recentTasks = tasks.items.slice(0, 5);
      },
      error: (error) => {
        console.error('Failed to load tasks', error);
      }
    });
  }

  getActiveTasksCount(): number {
    return this.tasks.filter(t => 
      ['IN_PROGRESS', 'BLOCKED', 'DELIVERING'].includes(t.status)
    ).length;
  }

  getCompletedTasksCount(): number {
    return this.tasks.filter(t => 
      ['FINISHED', 'CONFIRMED'].includes(t.status)
    ).length;
  }

  getHighPriorityCount(): number {
    return this.tasks.filter(t => t.priority === 'HIGH').length;
  }
}