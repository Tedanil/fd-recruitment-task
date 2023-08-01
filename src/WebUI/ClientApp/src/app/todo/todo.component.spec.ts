import { TestBed, async } from '@angular/core/testing';
import { TodoComponent } from './todo.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { ReactiveFormsModule } from '@angular/forms'; 

describe('TodoComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        TodoComponent
      ],
      imports: [
        HttpClientTestingModule,
        ModalModule.forRoot(),
        ReactiveFormsModule 
      ],
      providers: [
        BsModalService
      ]
    }).compileComponents();
  }));

  it('should create the todo component', () => {
    const fixture = TestBed.createComponent(TodoComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  });
});
