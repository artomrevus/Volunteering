import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { BindingService } from '../../core/services/binding.service';
import { TaskService } from '../../core/services/task.service';
import { BindingDto, CreateBindingDto, UpdateBindingDto, TaskDto } from '../../core/models/interfaces';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="profile-container">
      <div class="profile-header">
        <h1>User Profile</h1>
      </div>

      <div class="profile-content">
        <!-- User Information Section -->
        <div class="section-card">
          <h2>Account Information</h2>
          <div class="info-grid">
            <div class="info-item">
              <span class="label">Username:</span>
              <span class="value">{{ user()?.username }}</span>
            </div>
            <div class="info-item">
              <span class="label">User ID:</span>
              <span class="value">{{ user()?.id }}</span>
            </div>
            <div class="info-item">
              <span class="label">Role:</span>
              <span class="role-badge" [class]="'role-' + user()?.role?.toLowerCase()">
                {{ user()?.role }}
              </span>
            </div>
          </div>
        </div>

        <!-- Email Notification Binding Section -->
        <div class="section-card">
          <h2>Email Notifications</h2>
          
          @if (binding) {
            <div class="binding-info">
              <div class="info-item">
                <span class="label">Current Email:</span>
                <span class="value">{{ binding.email }}</span>
              </div>
              
              @if (!editingBinding) {
                <div class="binding-actions">
                  <button (click)="startEditBinding()" class="btn btn-primary">
                    Update Email
                  </button>
                  <button (click)="deleteBinding()" class="btn btn-danger">
                    Remove Email
                  </button>
                </div>
              } @else {
                <form (ngSubmit)="updateBinding()" class="email-form">
                  <div class="form-group">
                    <input
                      type="email"
                      [(ngModel)]="updateBindingDto.email"
                      name="email"
                      placeholder="Enter new email"
                      required
                      class="input"
                    />
                  </div>
                  <div class="form-actions">
                    <button type="button" (click)="cancelEditBinding()" class="btn btn-secondary">
                      Cancel
                    </button>
                    <button type="submit" class="btn btn-primary">
                      Update
                    </button>
                  </div>
                </form>
              }
            </div>
          } @else {
            <div class="no-binding">
              <p>No email configured for notifications</p>
              
              @if (!addingBinding) {
                <button (click)="startAddBinding()" class="btn btn-primary">
                  Add Email
                </button>
              } @else {
                <form (ngSubmit)="createBinding()" class="email-form">
                  <div class="form-group">
                    <input
                      type="email"
                      [(ngModel)]="createBindingDto.email"
                      name="email"
                      placeholder="Enter your email"
                      required
                      class="input"
                    />
                  </div>
                  <div class="form-actions">
                    <button type="button" (click)="cancelAddBinding()" class="btn btn-secondary">
                      Cancel
                    </button>
                    <button type="submit" class="btn btn-primary">
                      Add Email
                    </button>
                  </div>
                </form>
              }
            </div>
          }

          @if (bindingError) {
            <div class="error-message">{{ bindingError }}</div>
          }
        </div>

        <!-- Statistics Section -->
        <div class="section-card">
          <h2>Activity Statistics</h2>
          <div class="stats-grid">
            <div class="stat-card">
              <h3>Total Tasks</h3>
              <p class="stat-number">{{ statistics.total }}</p>
            </div>
            <div class="stat-card">
              <h3>Active Tasks</h3>
              <p class="stat-number">{{ statistics.active }}</p>
            </div>
            <div class="stat-card">
              <h3>Completed Tasks</h3>
              <p class="stat-number">{{ statistics.completed }}</p>
            </div>
            @if (user()?.role === 'MILITARY') {
              <div class="stat-card">
                <h3>Pending Confirmation</h3>
                <p class="stat-number">{{ statistics.pendingConfirmation }}</p>
              </div>
            }
          </div>
        </div>

        <!-- Recent Activity Section -->
        <div class="section-card">
          <h2>Recent Activity</h2>
          @if (recentTasks.length > 0) {
            <div class="activity-list">
              @for (task of recentTasks; track task.id) {
                <div class="activity-item">
                  <div class="activity-header">
                    <h4>{{ task.title }}</h4>
                    <span class="status-badge" [class]="'status-' + task.status.toLowerCase()">
                      {{ task.status }}
                    </span>
                  </div>
                  <p class="activity-date">{{ task.createdAt | date:'medium' }}</p>
                  <div class="activity-meta">
                    <span class="priority-badge" [class]="'priority-' + task.priority.toLowerCase()">
                      {{ task.priority }}
                    </span>
                    @if (task.volunteerId && user()?.role === 'MILITARY') {
                      <span class="volunteer-info">Volunteer: {{ task.volunteerId }}</span>
                    }
                    @if (task.militaryId && user()?.role === 'VOLUNTEER') {
                      <span class="military-info">Military: {{ task.militaryId }}</span>
                    }
                  </div>
                </div>
              }
            </div>
          } @else {
            <p class="no-activity">No recent activity</p>
          }
        </div>

        <!-- Danger Zone Section -->
        <div class="section-card danger-zone">
          <h2>Danger Zone</h2>
          <div class="danger-content">
            <div class="danger-item">
              <div>
                <h3>Logout</h3>
                <p>Sign out of your account</p>
              </div>
              <button (click)="logout()" class="btn btn-danger">
                Logout
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .profile-container {
      padding: 20px;
      max-width: 1000px;
      margin: 0 auto;
    }

    .profile-header {
      margin-bottom: 30px;
    }

    .profile-header h1 {
      color: #333;
      margin: 0;
    }

    .profile-content {
      display: flex;
      flex-direction: column;
      gap: 30px;
    }

    .section-card {
      background: white;
      border-radius: 12px;
      padding: 30px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .section-card h2 {
      margin: 0 0 20px 0;
      color: #333;
      font-size: 20px;
    }

    .info-grid {
      display: grid;
      gap: 15px;
    }

    .info-item {
      display: flex;
      align-items: center;
      padding: 10px;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .info-item .label {
      font-weight: 600;
      color: #555;
      margin-right: 15px;
      min-width: 120px;
    }

    .info-item .value {
      color: #333;
      font-size: 16px;
    }

    .role-badge {
      padding: 6px 16px;
      border-radius: 20px;
      font-size: 14px;
      font-weight: 600;
      text-transform: uppercase;
    }

    .role-military {
      background: #e3f2fd;
      color: #1565c0;
    }

    .role-volunteer {
      background: #f3e5f5;
      color: #7b1fa2;
    }

    .binding-info {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }

    .binding-actions {
      display: flex;
      gap: 10px;
    }

    .no-binding {
      text-align: center;
      padding: 20px;
    }

    .no-binding p {
      color: #666;
      margin-bottom: 20px;
    }

    .email-form {
      margin-top: 20px;
    }

    .form-group {
      margin-bottom: 15px;
    }

    .input {
      width: 100%;
      padding: 12px;
      border: 1px solid #ddd;
      border-radius: 6px;
      font-size: 16px;
      transition: border-color 0.3s;
    }

    .input:focus {
      outline: none;
      border-color: #667eea;
    }

    .form-actions {
      display: flex;
      gap: 10px;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
    }

    .stat-card {
      background: #f8f9fa;
      padding: 20px;
      border-radius: 8px;
      text-align: center;
    }

    .stat-card h3 {
      margin: 0 0 10px 0;
      color: #666;
      font-size: 14px;
      font-weight: 500;
    }

    .stat-number {
      font-size: 32px;
      font-weight: bold;
      color: #333;
      margin: 0;
    }

    .activity-list {
      display: flex;
      flex-direction: column;
      gap: 15px;
    }

    .activity-item {
      padding: 15px;
      background: #f8f9fa;
      border-radius: 8px;
      border-left: 4px solid #667eea;
    }

    .activity-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 8px;
    }

    .activity-header h4 {
      margin: 0;
      color: #333;
    }

    .activity-date {
      color: #666;
      font-size: 14px;
      margin: 0 0 10px 0;
    }

    .activity-meta {
      display: flex;
      gap: 10px;
      align-items: center;
    }

    .volunteer-info, .military-info {
      font-size: 14px;
      color: #666;
    }

    .no-activity {
      text-align: center;
      color: #999;
      padding: 20px;
    }

    .priority-badge, .status-badge {
      padding: 4px 10px;
      border-radius: 12px;
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

    .danger-zone {
      border: 2px solid #fed7d7;
    }

    .danger-zone h2 {
      color: #c53030;
    }

    .danger-content {
      display: flex;
      flex-direction: column;
      gap: 15px;
    }

    .danger-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 15px;
      background: #fff5f5;
      border-radius: 8px;
    }

    .danger-item h3 {
      margin: 0 0 5px 0;
      color: #333;
    }

    .danger-item p {
      margin: 0;
      color: #666;
      font-size: 14px;
    }

    .btn {
      padding: 10px 20px;
      border: none;
      border-radius: 6px;
      font-weight: 600;
      cursor: pointer;
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

    .btn-danger {
      background: #f56565;
      color: white;
    }

    .btn-danger:hover {
      background: #e53e3e;
    }

    .error-message {
      background: #fee;
      color: #c33;
      padding: 12px;
      border-radius: 6px;
      margin-top: 15px;
    }
  `]
})
export class ProfileComponent implements OnInit {
  binding: BindingDto | null = null;
  createBindingDto: CreateBindingDto = { email: '' };
  updateBindingDto: UpdateBindingDto = { email: '' };
  addingBinding = false;
  editingBinding = false;
  bindingError = '';
  
  recentTasks: TaskDto[] = [];
  statistics = {
    total: 0,
    active: 0,
    completed: 0,
    pendingConfirmation: 0
  };

  constructor(
    public authService: AuthService,
    private bindingService: BindingService,
    private taskService: TaskService
  ) {}

  ngOnInit(): void {
    this.loadBinding();
    this.loadStatistics();
  }

  get user() {
    return this.authService.currentUser;
  }

  loadBinding(): void {
    this.bindingService.getBinding().subscribe({
      next: (binding) => {
        this.binding = binding;
      },
      error: (error) => {
        // No binding exists yet
        console.log('No binding found');
      }
    });
  }

  loadStatistics(): void {
    const user = this.user();
    if (!user) return;

    const filter = user.role === 'MILITARY' 
      ? { militaryId: user.id }
      : { volunteerId: user.id };

    this.taskService.filterTasks(filter).subscribe({
      next: (tasks) => {
        this.recentTasks = tasks.items.slice(0, 5);
        this.statistics.total = tasks.items.length;
        this.statistics.active = tasks.items.filter(t => 
          ['IN_PROGRESS', 'BLOCKED', 'DELIVERING'].includes(t.status)
        ).length;
        this.statistics.completed = tasks.items.filter(t => 
          ['FINISHED', 'CONFIRMED'].includes(t.status)
        ).length;
        this.statistics.pendingConfirmation = tasks.items.filter(t => 
          t.status === 'FINISHED'
        ).length;
      },
      error: (error) => {
        console.error('Failed to load statistics', error);
      }
    });
  }

  startAddBinding(): void {
    this.addingBinding = true;
    this.createBindingDto.email = '';
    this.bindingError = '';
  }

  cancelAddBinding(): void {
    this.addingBinding = false;
    this.createBindingDto.email = '';
    this.bindingError = '';
  }

  createBinding(): void {
    if (!this.createBindingDto.email) {
      this.bindingError = 'Please enter a valid email';
      return;
    }

    this.bindingError = '';
    this.bindingService.createBinding(this.createBindingDto).subscribe({
      next: (binding) => {
        this.binding = binding;
        this.addingBinding = false;
        this.createBindingDto.email = '';
      },
      error: (error) => {
        this.bindingError = error.error?.message || 'Failed to add email';
      }
    });
  }

  startEditBinding(): void {
    if (this.binding) {
      this.editingBinding = true;
      this.updateBindingDto.email = this.binding.email;
      this.bindingError = '';
    }
  }

  cancelEditBinding(): void {
    this.editingBinding = false;
    this.updateBindingDto.email = '';
    this.bindingError = '';
  }

  updateBinding(): void {
    if (!this.updateBindingDto.email) {
      this.bindingError = 'Please enter a valid email';
      return;
    }

    this.bindingError = '';
    this.bindingService.updateBinding(this.updateBindingDto).subscribe({
      next: (binding) => {
        this.binding = binding;
        this.editingBinding = false;
        this.updateBindingDto.email = '';
      },
      error: (error) => {
        this.bindingError = error.error?.message || 'Failed to update email';
      }
    });
  }

  deleteBinding(): void {
    if (confirm('Are you sure you want to remove your email? You will stop receiving notifications.')) {
      this.bindingService.deleteBinding().subscribe({
        next: () => {
          this.binding = null;
          this.bindingError = '';
        },
        error: (error) => {
          this.bindingError = error.error?.message || 'Failed to remove email';
        }
      });
    }
  }

  logout(): void {
    if (confirm('Are you sure you want to logout?')) {
      this.authService.logout();
      window.location.href = '/login';
    }
  }
}