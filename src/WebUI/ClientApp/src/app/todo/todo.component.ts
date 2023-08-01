import { Component, TemplateRef, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';


import {
  TodoListsClient, TodoItemsClient,
  TodoListDto, TodoItemDto, PriorityLevelDto,
  CreateTodoListCommand, UpdateTodoListCommand,
  CreateTodoItemCommand, UpdateTodoItemDetailCommand, TodoItemTagClient, CreateTagCommand, TagDto, TagsClient, TodosByTagInListVm
} from '../web-api-client';
import { Color } from '../models/color';
import { Observable } from 'rxjs';


@Component({
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  debug = false;
  deleting = false;
  deleteCountDown = 0;
  deleteCountDownInterval: any;
  lists: TodoListDto[];
  priorityLevels: PriorityLevelDto[];
  selectedList: TodoListDto;
  staticSelectedList: TodoListDto;
  selectedItem: TodoItemDto;
  newListEditor: any = {};
  listOptionsEditor: any = {};
  newListModalRef: BsModalRef;
  colors = Color.SupportedColors;
  selectedColor = 'white';
  newTag: string;
  tags$: Observable<TagDto[]>;
  topTags: TagDto[];
  selectedTag: string = '';
  allTags: TagDto[];
  todoItems: TodoItemDto[];
  listOptionsModalRef: BsModalRef;
  deleteListModalRef: BsModalRef;
  itemDetailsModalRef: BsModalRef;
  itemDetailsFormGroup = this.fb.group({
    id: [null],
    listId: [null],
    priority: [''],
    note: [''],
    color: ['#FFFFFF'],
    newTag: [''],
  });


  constructor(
    private listsClient: TodoListsClient,
    private itemsClient: TodoItemsClient,
    private modalService: BsModalService,
    private tagsClient: TodoItemTagClient,
    private tagsClient2: TagsClient,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.listsClient.get().subscribe(
      result => {
        this.lists = result.lists;
        this.priorityLevels = result.priorityLevels;
        if (this.lists.length) {
          this.selectedList = this.lists[0];
          // deep copy
          if (this.selectedList) {
            this.staticSelectedList = TodoListDto.fromJS(this.selectedList);
          }
        }
      },
      error => console.error(error)
    );
    this.itemDetailsFormGroup.get('id').valueChanges.subscribe(
      todoItemId => {
        this.getTagsForItem(todoItemId);
      }
    );
    this.getTopTags();
    this.getAllTags();
  }



  getColorName(colorCode: string): string {
    return Color.colorNames[colorCode];
  }

  // Lists
  remainingItems(list: TodoListDto): number {
    return list.items.filter(t => !t.done).length;
  }

  showNewListModal(template: TemplateRef<any>): void {
    this.newListModalRef = this.modalService.show(template);
    setTimeout(() => document.getElementById('title').focus(), 250);
  }

  newListCancelled(): void {
    this.newListModalRef.hide();
    this.newListEditor = {};
  }

  addList(): void {
    const list = {
      id: 0,
      title: this.newListEditor.title,
      items: []
    } as TodoListDto;

    this.listsClient.create(list as CreateTodoListCommand).subscribe(
      result => {
        list.id = result;
        this.lists.push(list);
        this.selectedList = list;
        this.newListModalRef.hide();
        this.newListEditor = {};
      },
      error => {
        const errors = JSON.parse(error.response);

        if (errors && errors.Title) {
          this.newListEditor.error = errors.Title[0];
        }

        setTimeout(() => document.getElementById('title').focus(), 250);
      }
    );
  }

  showListOptionsModal(template: TemplateRef<any>) {
    this.listOptionsEditor = {
      id: this.selectedList.id,
      title: this.selectedList.title
    };

    this.listOptionsModalRef = this.modalService.show(template);
  }

  updateListOptions() {
    const list = this.listOptionsEditor as UpdateTodoListCommand;
    this.listsClient.update(this.selectedList.id, list).subscribe(
      () => {
        (this.selectedList.title = this.listOptionsEditor.title),
          this.listOptionsModalRef.hide();
        this.listOptionsEditor = {};
      },
      error => console.error(error)
    );
  }

  confirmDeleteList(template: TemplateRef<any>) {
    this.listOptionsModalRef.hide();
    this.deleteListModalRef = this.modalService.show(template);
  }

  deleteListConfirmed(): void {
    this.listsClient.delete(this.selectedList.id).subscribe(
      () => {
        this.deleteListModalRef.hide();
        this.lists = this.lists.filter(t => t.id !== this.selectedList.id);
        this.selectedList = this.lists.length ? this.lists[0] : null;
      },
      error => console.error(error)
    );
  }

  // Items
  showItemDetailsModal(template: TemplateRef<any>, item: TodoItemDto): void {
    this.selectedItem = item;
    this.itemDetailsFormGroup.patchValue(this.selectedItem);

    this.itemDetailsModalRef = this.modalService.show(template);
    this.itemDetailsModalRef.onHidden.subscribe(() => {
      this.stopDeleteCountDown();
    });
  }

  updateItemDetails(): void {
    const item = new UpdateTodoItemDetailCommand(this.itemDetailsFormGroup.value);
    this.itemsClient.updateItemDetails(this.selectedItem.id, item).subscribe(
      () => {
        if (this.selectedItem.listId !== item.listId) {
          this.selectedList.items = this.selectedList.items.filter(
            i => i.id !== this.selectedItem.id
          );
          const listIndex = this.lists.findIndex(
            l => l.id === item.listId
          );
          this.selectedItem.listId = item.listId;
          this.lists[listIndex].items.push(this.selectedItem);
        }

        this.selectedItem.priority = item.priority;
        this.selectedItem.note = item.note;
        this.selectedItem.color = item.color;
        this.itemDetailsModalRef.hide();
        this.itemDetailsFormGroup.reset();
      },
      error => console.error(error)
    );
  }

  addItem() {
    const item = {
      id: 0,
      listId: this.selectedList.id,
      priority: this.priorityLevels[0].value,
      title: '',
      done: false
    } as TodoItemDto;

    this.selectedList.items.push(item);
    const index = this.selectedList.items.length - 1;
    this.editItem(item, 'itemTitle' + index);
  }

  editItem(item: TodoItemDto, inputId: string): void {
    this.selectedItem = item;
    setTimeout(() => document.getElementById(inputId).focus(), 100);
  }

  updateItem(item: TodoItemDto, pressedEnter: boolean = false): void {
    const isNewItem = item.id === 0;

    if (!item.title.trim()) {
      this.deleteItem(item);
      return;
    }

    if (item.id === 0) {
      this.itemsClient
        .create({
          ...item, listId: this.selectedList.id
        } as CreateTodoItemCommand)
        .subscribe(
          result => {
            item.id = result;
          },
          error => console.error(error)
        );
    } else {
      this.itemsClient.update(item.id, item).subscribe(
        () => console.log('Update succeeded.'),
        error => console.error(error)
      );
    }

    this.selectedItem = null;

    if (isNewItem && pressedEnter) {
      setTimeout(() => this.addItem(), 250);
    }
  }

  deleteItem(item: TodoItemDto, countDown?: boolean) {
    if (countDown) {
      if (this.deleting) {
        this.stopDeleteCountDown();
        return;
      }
      this.deleteCountDown = 3;
      this.deleting = true;
      this.deleteCountDownInterval = setInterval(() => {
        if (this.deleting && --this.deleteCountDown <= 0) {
          this.deleteItem(item, false);
        }
      }, 1000);
      return;
    }
    this.deleting = false;
    if (this.itemDetailsModalRef) {
      this.itemDetailsModalRef.hide();
    }

    if (item.id === 0) {
      const itemIndex = this.selectedList.items.indexOf(this.selectedItem);
      this.selectedList.items.splice(itemIndex, 1);
    } else {
      this.itemsClient.delete(item.id).subscribe(
        () =>
        (this.selectedList.items = this.selectedList.items.filter(
          t => t.id !== item.id
        )),
        error => console.error(error)
      );
    }
  }

  stopDeleteCountDown() {
    clearInterval(this.deleteCountDownInterval);
    this.deleteCountDown = 0;
    this.deleting = false;
  }

  addTagToItem(todoItemId: number, tagName: string) {
    const command = new CreateTagCommand();
    command.tagName = tagName;

    this.tagsClient.addTagToTodoItem(todoItemId, command).subscribe(
      result => {
        console.log("Tag successfully added");
        this.getTagsForItem(todoItemId);
        this.itemDetailsFormGroup.get('newTag').setValue('');
        this.getAllTags();
      },
      error => {
        console.error(error);
      }
    );
  }

  getTagsForItem(todoItemId: number) {
    this.tags$ = this.tagsClient.getTagsForTodoItem(todoItemId);
  }

  deleteTag(todoItemId: number, tagId: number): void {
    this.tagsClient.removeTagFromTodoItem(todoItemId, tagId)
      .subscribe(
        response => {
          console.log(response);
          this.getTagsForItem(todoItemId);
        },
        error => console.error(error)
      );
  }

  getTopTags(): void {
    this.tagsClient2.getTopTags().subscribe(data => {
      if (data && data.tags) {
        this.topTags = data.tags;
        console.log(data)
      } else {
        console.error('No tags received');
      }
    }, error => console.error('HTTP request error: ', error));
  }

  getAllTags(): void {
    this.tagsClient2.getAllTags().subscribe(
      (response) => {
        this.allTags = response.tags;
        console.log(this.allTags);
      },
      (error) => {
        console.error('Error occurred while fetching all tags:', error);
      }
    );
  }

  filterItemsByTag(): void {
    if (!this.selectedTag || this.selectedTag === '') {
      this.selectedList.items = this.staticSelectedList.items;
      return;
    }

    if (!this.selectedList) return;

    const tagId = Number(this.selectedTag);
    console.log(tagId)

    this.itemsClient.getTodoItemsByTagInList(tagId, this.selectedList.id).subscribe(
      (response: TodosByTagInListVm) => {
        this.todoItems = response.items;
        console.log(this.todoItems);
        this.selectedList.items = this.todoItems;
      },
      error => {
        console.error(error);
      }
    );
  }

  selectList(list: TodoListDto): void {
    this.selectedList = list;
    this.staticSelectedList = TodoListDto.fromJS(this.selectedList);
  }
}