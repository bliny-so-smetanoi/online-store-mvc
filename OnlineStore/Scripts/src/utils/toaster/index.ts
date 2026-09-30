import $ from 'jquery';
import Toast from 'bootstrap/js/dist/toast';

type ToastLevel = 'info' | 'success' | 'warning' | 'error'; 

export function showToast(title: string = '', body: string = '', level: ToastLevel = "success"): void {
    const toastLiveExample = document.getElementById('liveToast') as Element;
    const toastBody = toastLiveExample.querySelector('.toast-body') as HTMLElement;
    const toastTitle = toastLiveExample.querySelector('.me-auto') as HTMLElement;
    const toastHeader = toastLiveExample.querySelector('.toast-header') as HTMLElement;
    toastBody.textContent = body;
    toastTitle.textContent = title;
    toastHeader.classList.add(getLevelClass(level));
    console.log(toastHeader);
    const toastBootstrap = Toast.getOrCreateInstance(toastLiveExample);
    toastBootstrap.show();
}

function getLevelClass(level: ToastLevel): string {
    if (level === 'warning'){
        return 'text-bg-warning';
    }
    if (level === 'info'){
        return 'text-bg-info';
    }
    if (level === 'success'){
        return 'text-bg-success';
    }
    if (level === 'error'){
        return 'text-bg-danger';
    }
    return 'text-bg-warning';
}
