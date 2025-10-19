import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TaskService } from '../../../core/services/task.service';
import { CreateTaskDto } from '../../../core/models/interfaces';

@Component({
  selector: 'app-task-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="create-task-container">
      <div class="form-card">
        <h1>Create New Task</h1>
        
        <form (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label for="title">Title *</label>
            <input
              type="text"
              id="title"
              name="title"
              [(ngModel)]="task.title"
              required
              minlength="3"
              maxlength="200"
              class="input"
              placeholder="Enter task title"
            />
            <span class="hint">3-200 characters</span>
          </div>

          <div class="form-group">
            <label for="description">Description *</label>
            <textarea
              id="description"
              name="description"
              [(ngModel)]="task.description"
              required
              minlength="3"
              maxlength="1000"
              rows="6"
              class="input"
              placeholder="Enter task description"
            ></textarea>
            <span class="hint">3-1000 characters</span>
          </div>

          <div class="form-group">
            <label for="priority">Priority *</label>
            <select
              id="priority"
              name="priority"
              [(ngModel)]="task.priority"
              required
              class="input"
            >
              <option value="">Select priority</option>
              <option value="LOW">Low</option>
              <option value="AVERAGE">Average</option>
              <option value="HIGH">High</option>
            </select>
          </div>

          @if (error) {
            <div class="error-message">{{ error }}</div>
          }

          <div class="form-actions">
            <button type="button" (click)="cancel()" class="btn btn-secondary">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary" [disabled]="loading">
              {{ loading ? 'Creating...' : 'Create Task' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .create-task-container {
      padding: 20px;
      max-width: 800px;
      margin: 0 auto;
    }

    .form-card {
      background: white;
      border-radius: 12px;
      padding: 40px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    h1 {
      margin-bottom: 30px;
      color: #333;
    }

    .form-group {
      margin-bottom: 25px;
    }

    label {
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

    .hint {
      display: block;
      margin-top: 4px;
      font-size: 13px;
      color: #999;
    }

    .error-message {
      background: #fee;
      color: #c33;
      padding: 12px;
      border-radius: 6px;
      margin-bottom: 20px;
    }

    .form-actions {
      display: flex;
      gap: 15px;
      justify-content: flex-end;
      margin-top: 30px;
    }

    .btn {
      padding: 12px 24px;
      border: none;
      border-radius: 6px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
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

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
  `]
})
export class TaskCreateComponent {
  task: CreateTaskDto = {
    title: '',
    description: '',
    priority: '' as 'LOW' | 'AVERAGE' | 'HIGH'
  };
  loading = false;
  error = '';

  constructor(
    private taskService: TaskService,
    private router: Router
  ) {}

  onSubmit(): void {
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;
    this.error = '';

    this.taskService.createTask(this.task).subscribe({
      next: (createdTask) => {
        this.router.navigate(['/tasks', createdTask.id]);
      },
      error: (error) => {
        this.error = error.error?.message || 'Failed to create task. Please try again.';
        this.loading = false;
      }
    });
  }

  validateForm(): boolean {
    if (!this.task.title || this.task.title.length < 3 || this.task.title.length > 200) {
      this.error = 'Title must be between 3 and 200 characters';
      return false;
    }

    if (!this.task.description || this.task.description.length < 3 || this.task.description.length > 1000) {
      this.error = 'Description must be between 3 and 1000 characters';
      return false;
    }

    if (!this.task.priority) {
      this.error = 'Please select a priority';
      return false;
    }

    return true;
  }

  cancel(): void {
    this.router.navigate(['/tasks']);
  }
}