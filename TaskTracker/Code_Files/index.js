// Fetch all elements needed for the functionality to work
const taskInput = document.getElementById("taskInput");
const addTaskBtn = document.getElementById("addTaskBtn");
const taskList = document.getElementById("taskList");
const totalTasksSpan = document.getElementById("totalTasks");
const completedTasksSpan = document.getElementById("completedTasks");

// Making add task button listen to click and execute add task function
addTaskBtn.addEventListener("click", addTask);


function updateTasksCount() {
    const totalTasks = taskList.children.length;

    // Completed tasks = number of ticked checkboxes
    const completedTasks = taskList.querySelectorAll("input[type=checkbox]:checked").length;

    // update spans with the respective values
    totalTasksSpan.textContent = totalTasks;
    completedTasksSpan.textContent = completedTasks;
}

function addTask() {
    // fetch the text from the input area
    const taskText = taskInput.value.trim();
    if(taskText == "") return;

    // create a list item
    const li = document.createElement("li");

    // create a checkbox for the list item
    const checkbox = document.createElement("input");
    checkbox.type = "checkbox";

    // create a span for list item description
    const span = document.createElement("span");
    span.textContent = taskText;

    // create delete button for the list item
    const deletebButton = document.createElement("button");
    deletebButton.textContent = "Delete Task";
    deletebButton.classList.add("delbtn");

    // add functionality to checkbox
    checkbox.addEventListener("change", function () {
        span.classList.toggle("completed");
        updateTasksCount();
        checkbox.disabled = true;
    });

    // add functionality to delete button
    deletebButton.addEventListener("click", function () {
        li.remove();
        updateTasksCount();
    });

    // append all of them together to the task list when add button is clicked
    li.appendChild(checkbox);
    li.appendChild(span);
    li.appendChild(deletebButton);
    taskList.appendChild(li);
    updateTasksCount();
    taskInput.value("");
}