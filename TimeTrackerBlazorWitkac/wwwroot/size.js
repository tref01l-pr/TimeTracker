window.setElementHeight = (height) => {
    const element = document.querySelector('.scheduler');
    if (element) {
        element.style.height = `${height}px`
    }
}