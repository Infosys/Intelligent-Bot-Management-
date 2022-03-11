export class CustomRow {
  
  isChecked: boolean = true;

  constructor(protected data: any) {
  }

  getData(): any {
    return this.data;
  }
}
