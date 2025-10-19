import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { TaskDto, UpdateTaskDto, UpdateTaskStatusDto } from '../../../core/models/interfaces';

@Component({
  selector: 'app-task-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="task-detail-container">
      @if (loading) {
        <div class="loading">Loading task details...</div>
      } @else if (task) {
        <div class="task-detail-card">
          <div class="task-header">
            <div>
              <h1>{{ task.title }}</h1>
              <div class="task-badges">
                <span class="priority-badge" [class]="'priority-' + task.priority.toLowerCase()">
                  {{ task.priority }} PRIORITY
                </span>
                <span class="status-badge" [class]="'status-' + task.status.toLowerCase()">
                  {{ task.status }}
                </span>
              </div>
            </div>
            <div class="task-actions">
              @if (canEditTask()) {
                <button (click)="toggleEditMode()" class="btn btn-primary">
                  {{ editMode ? 'Cancel Edit' : 'Edit Task' }}
                </button>
              }
              @if (canStartTask()) {
                <button (click)="startTask()" class="btn btn-success">Start Task</button>
              }
              @if (canConfirmTask()) {
                <button (click)="confirmTask()" class="btn btn-success">Confirm Completion</button>
              }
            </div>
          </div>

          @if (editMode) {
            <form (ngSubmit)="updateTask()" class="edit-form">
              <div class="form-group">
                <label for="title">Title</label>
                <input
                  type="text"
                  id="title"
                  [(ngModel)]="editDto.title"
                  name="title"
                  required
                  minlength="3"
                  maxlength="200"
                  class="input"
                />
              </div>
              
              <div class="form-group">
                <label for="description">Description</label>
                <textarea
                  id="description"
                  [(ngModel)]="editDto.description"
                  name="description"
                  required
                  minlength="3"
                  maxlength="1000"
                  rows="6"
                  class="input"
                ></textarea>
              </div>
              
              <div class="form-group">
                <label for="priority">Priority</label>
                <select
                  id="priority"
                  [(ngModel)]="editDto.priority"
                  name="priority"
                  required
                  class="input"
                >
                  <option value="LOW">Low</option>
                  <option value="AVERAGE">Average</option>
                  <option value="HIGH">High</option>
                </select>
              </div>
              
              <div class="form-actions">
                <button type="button" (click)="toggleEditMode()" class="btn btn-secondary">
                  Cancel
                </button>
                <button type="submit" class="btn btn-primary">Save Changes</button>
              </div>
            </form>
          } @else {
            <div class="task-content">
              <section class="description-section">
                <h2>Description</h2>
                <p>{{ task.description }}</p>
              </section>

              <section class="details-section">
                <h2>Task Details</h2>
                <div class="details-grid">
                  <div class="detail-item">
                    <span class="label">Task ID:</span>
                    <span class="value">{{ task.id }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="label">Military Unit ID:</span>
                    <span class="value">{{ task.militaryId }}</span>
                  </div>
                  @if (task.volunteerId) {
                    <div class="detail-item">
                      <span class="label">Volunteer ID:</span>
                      <span class="value">{{ task.volunteerId }}</span>
                    </div>
                  }
                  <div class="detail-item">
                    <span class="label">Created:</span>
                    <span class="value">{{ task.createdAt | date:'medium' }}</span>
                  </div>
                  @if (task.startedAt) {
                    <div class="detail-item">
                      <span class="label">Started:</span>
                      <span class="value">{{ task.startedAt | date:'medium' }}</span>
                    </div>
                  }
                  @if (task.finishedAt) {
                    <div class="detail-item">
                      <span class="label">Finished:</span>
                      <span class="value">{{ task.finishedAt | date:'medium' }}</span>
                    </div>
                  }
                  @if (task.confirmedAt) {
                    <div class="detail-item">
                      <span class="label">Confirmed:</span>
                      <span class="value">{{ task.confirmedAt | date:'medium' }}</span>
                    </div>
                  }
                </div>
              </section>

              @if (canUpdateStatus()) {
                <section class="status-update-section">
                  <h2>Update Status</h2>
                  <div class="status-update-form">
                    <select [(ngModel)]="newStatus" class="input">
                      <option value="">Select new status</option>
                      <option value="IN_PROGRESS">In Progress</option>
                      <option value="BLOCKED">Blocked</option>
                      <option value="DELIVERING">Delivering</option>
                      <option value="FINISHED">Finished</option>
                    </select>
                    <button 
                      (click)="updateStatus()" 
                      [disabled]="!newStatus"
                      class="btn btn-primary"
                    >
                      Update Status
                    </button>
                  </div>
                </section>
              }

              <section class="timeline-section">
                <h2>Task Timeline</h2>
                <div class="timeline">
                  <div class="timeline-item" [class.completed]="true">
                    <div class="timeline-marker"></div>
                    <div class="timeline-content">
                      <h4>Created</h4>
                      <p>{{ task.createdAt | date:'medium' }}</p>
                    </div>
                  </div>
                  
                  @if (task.startedAt) {
                    <div class="timeline-item completed">
                      <div class="timeline-marker"></div>
                      <div class="timeline-content">
                        <h4>Started</h4>
                        <p>{{ task.startedAt | date:'medium' }}</p>
                        <p class="volunteer">By Volunteer: {{ task.volunteerId }}</p>
                      </div>
                    </div>
                  }
                  
                  @if (task.status === 'BLOCKED') {
                    <div class="timeline-item blocked">
                      <div class="timeline-marker"></div>
                      <div class="timeline-content">
                        <h4>Blocked</h4>
                        <p>Task is currently blocked</p>
                      </div>
                    </div>
                  }
                  
                  @if (task.status === 'DELIVERING') {
                    <div class="timeline-item delivering">
                      <div class="timeline-marker"></div>
                      <div class="timeline-content">
                        <h4>Delivering</h4>
                        <p>Task is being delivered</p>
                      </div>
                    </div>
                  }
                  
                  @if (task.finishedAt) {
                    <div class="timeline-item completed">
                      <div class="timeline-marker"></div>
                      <div class="timeline-content">
                        <h4>Finished</h4>
                        <p>{{ task.finishedAt | date:'medium' }}</p>
                      </div>
                    </div>
                  }
                  
                  @if (task.confirmedAt) {
                    <div class="timeline-item confirmed">
                      <div class="timeline-marker"></div>
                      <div class="timeline-content">
                        <h4>Confirmed</h4>
                        <p>{{ task.confirmedAt | date:'medium' }}</p>
                        <p>Confirmed by Military Unit</p>
                      </div>
                    </div>
                  }
                </div>
              </section>
            </div>
          }

          @if (error) {
            <div class="error-message">{{ error }}</div>
          }
        </div>
      } @else {
        <div class="not-found">
          <h2>Task not found</h2>
          <a routerLink="/tasks" class="btn btn-primary">Back to Tasks</a>
        </div>
      }
    </div>
  `,
  styles: [`
    .task-detail-container {
      padding: 20px;
      max-width: 1000px;
      margin: 0 auto;
    }

    .loading, .not-found {
      text-align: center;
      padding: 60px;
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .task-detail-card {
      background: white;
      border-radius: 12px;
      padding: 30px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .task-header {
      display: flex;
      justify-content: space-between;
      align-items: start;
      margin-bottom: 30px;
      padding-bottom: 20px;
      border-bottom: 2px solid #f0f0f0;
    }

    .task-header h1 {
      margin: 0 0 15px 0;
      color: #333;
    }

    .task-badges {
      display: flex;
      gap: 10px;
    }

    .priority-badge, .status-badge {
      padding: 6px 16px;
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

    .task-actions {
      display: flex;
      gap: 10px;
    }

    .task-content section {
      margin-bottom: 40px;
    }

    .task-content h2 {
      color: #333;
      margin-bottom: 20px;
      font-size: 20px;
    }

    .description-section p {
      color: #666;
      line-height: 1.6;
      font-size: 16px;
    }

    .details-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 15px;
    }

    .detail-item {
      display: flex;
      padding: 10px;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .detail-item .label {
      font-weight: 600;
      color: #555;
      margin-right: 10px;
      min-width: 120px;
    }

    .detail-item .value {
      color: #333;
      word-break: break-all;
    }

    .status-update-section {
      background: #f8f9fa;
      padding: 20px;
      border-radius: 8px;
    }

    .status-update-form {
      display: flex;
      gap: 15px;
      align-items: center;
    }

    .status-update-form select {
      flex: 1;
      max-width: 300px;
    }

    .timeline {
      position: relative;
      padding-left: 40px;
    }

    .timeline::before {
      content: '';
      position: absolute;
      left: 15px;
      top: 0;
      bottom: 0;
      width: 2px;
      background: #e2e8f0;
    }

    .timeline-item {
      position: relative;
      margin-bottom: 30px;
    }

    .timeline-marker {
      position: absolute;
      left: -30px;
      top: 5px;
      width: 12px;
      height: 12px;
      border-radius: 50%;
      background: #e2e8f0;
      border: 2px solid white;
      box-shadow: 0 0 0 4px #f8f9fa;
    }

    .timeline-item.completed .timeline-marker {
      background: #48bb78;
    }

    .timeline-item.blocked .timeline-marker {
      background: #f56565;
    }

    .timeline-item.delivering .timeline-marker {
      background: #ed64a6;
    }

    .timeline-item.confirmed .timeline-marker {
      background: #38b2ac;
    }

    .timeline-content h4 {
      margin: 0 0 5px 0;
      color: #333;
    }

    .timeline-content p {
      margin: 0;
      color: #666;
      font-size: 14px;
    }

    .timeline-content .volunteer {
      margin-top: 5px;
      font-style: italic;
    }

    .edit-form {
      margin-top: 30px;
    }

    .form-group {
      margin-bottom: 20px;
    }

    .form-group label {
      display: block;
      margin-bottom: 8px;
      color: #555;
      font-weight: 500;
    }

    .input {
      width: 100%;
      padding: 12px;
      border: 1px solid #ddd;
      border-radius: 6px;
      font-size: 16px;
      transition: border-color 0.3s;
      font-family: inherit;
    }

    .input:focus {
      outline: none;
      border-color: #667eea;
    }

    textarea.input {
      resize: vertical;
      min-height: 120px;
    }

    .form-actions {
      display: flex;
      gap: 15px;
      justify-content: flex-end;
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

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
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

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .error-message {
      background: #fee;
      color: #c33;
      padding: 12px;
      border-radius: 6px;
      margin-top: 20px;
    }
  `]
})
export class TaskDetailComponent implements OnInit {
  task: TaskDto | null = null;
  loading = true;
  error = '';
  editMode = false;
  editDto: UpdateTaskDto = {
    taskId: '',
    title: '',
    description: '',
    priority: 'AVERAGE'
  };
  newStatus: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private taskService: TaskService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadTask(id);
    }
  }

  get user() {
    return this.authService.currentUser();
  }

  loadTask(id: string): void {
    this.loading = true;
    this.taskService.getTask(id).subscribe({
      next: (task) => {
        this.task = task;
        this.editDto = {
          taskId: task.id,
          title: task.title,
          description: task.description,
          priority: task.priority
        };
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load task', error);
        this.loading = false;
      }
    });
  }

  canEditTask(): boolean {
    const user = this.user;
    return !!(user?.role === 'MILITARY' && 
             this.task?.militaryId === user.id && 
             this.task?.status === 'CREATED');
  }

  canStartTask(): boolean {
    const user = this.user;
    return !!(user?.role === 'VOLUNTEER' && 
             this.task?.status === 'CREATED');
  }

  canUpdateStatus(): boolean {
    const user = this.user;
    return !!(user?.role === 'VOLUNTEER' && 
             this.task?.volunteerId === user.id && 
             ['IN_PROGRESS', 'BLOCKED', 'DELIVERING'].includes(this.task?.status || ''));
  }

  canConfirmTask(): boolean {
    const user = this.user;
    return !!(user?.role === 'MILITARY' && 
             this.task?.militaryId === user.id && 
             this.task?.status === 'FINISHED');
  }

  toggleEditMode(): void {
    this.editMode = !this.editMode;
    if (this.editMode && this.task) {
      this.editDto = {
        taskId: this.task.id,
        title: this.task.title,
        description: this.task.description,
        priority: this.task.priority
      };
    }
  }

  updateTask(): void {
    this.error = '';
    this.taskService.updateTask(this.editDto).subscribe({
      next: (updatedTask) => {
        this.task = updatedTask;
        this.editMode = false;
      },
      error: (error) => {
        this.error = error.error?.message || 'Failed to update task';
      }
    });
  }

  startTask(): void {
    if (!this.task) return;
    
    this.error = '';
    this.taskService.startTask({ taskId: this.task.id }).subscribe({
      next: (updatedTask) => {
        this.task = updatedTask;
      },
      error: (error) => {
        this.error = error.error?.message || 'Failed to start task';
      }
    });
  }

  updateStatus(): void {
    if (!this.task || !this.newStatus) return;
    
    this.error = '';
    const dto: UpdateTaskStatusDto = {
      taskId: this.task.id,
      status: this.newStatus as any
    };
    
    this.taskService.updateTaskStatus(dto).subscribe({
      next: (updatedTask) => {
        this.task = updatedTask;
        this.newStatus = '';
      },
      error: (error) => {
        this.error = error.error?.message || 'Failed to update task status';
      }
    });
  }

  confirmTask(): void {
    if (!this.task) return;
    
    this.error = '';
    this.taskService.confirmTask({ taskId: this.task.id }).subscribe({
      next: (updatedTask) => {
        this.task = updatedTask;
      },
      error: (error) => {
        this.error = error.error?.message || 'Failed to confirm task';
      }
    });
  }
}