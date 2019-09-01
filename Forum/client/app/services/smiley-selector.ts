﻿import { insertAtCaret, show, hide } from '../helpers';

export class SmileySelector {
	private win: Window;
	private body: HTMLBodyElement;
	private smileySelector: HTMLElement;
	private smileySelectorImageHandler: ((event: Event) => void) = () => {};
	private smileySelectorTargetTextArea: HTMLTextAreaElement | null = null;

	constructor(private doc: Document) {
		this.win = <Window>doc.defaultView;
		this.body = doc.getElementsByTagName('body')[0];
		this.smileySelector = <HTMLElement>doc.querySelector('#smiley-selector');
	}

	// Used in message forms to insert smileys into textareas.
	init(): void {
		this.doc.querySelectorAll('.add-smiley').forEach(element => {
			element.removeEventListener('click', this.eventShowSmileySelector);
			element.addEventListener('click', this.eventShowSmileySelector);
		});
	}

	showSmileySelectorNearElement(target: HTMLElement, imageHandler: (event: Event) => void): void {
		let self = this;
		self.eventCloseSmileySelector();
		self.smileySelectorImageHandler = imageHandler;

		if (self.smileySelectorImageHandler) {
			self.smileySelector.querySelectorAll('img').forEach(element => {
				element.addEventListener('click', self.smileySelectorImageHandler);
			});
		}

		var rect = target.getBoundingClientRect();
		var targetTop = rect.top + self.win.pageYOffset - (<HTMLElement>self.doc.documentElement).clientTop;
		var targetLeft = rect.left + self.win.pageXOffset - (<HTMLElement>self.doc.documentElement).clientLeft;

		show(self.smileySelector);
		self.smileySelector.addEventListener('click', self.eventStopPropagation);

		let selectorTopOffset = targetTop + rect.height;
		self.smileySelector.style.top = selectorTopOffset + (selectorTopOffset == 0 ? '' : 'px');

		let selectorLeftOffset = targetLeft;
		var screenFalloff = targetLeft + self.smileySelector.clientWidth + 20 - self.win.innerWidth;

		if (screenFalloff > 0) {
			selectorLeftOffset -= screenFalloff;
		}

		self.smileySelector.style.left = selectorLeftOffset + (selectorLeftOffset == 0 ? '' : 'px');

		setTimeout(function () {
			self.body.addEventListener('click', self.eventCloseSmileySelector);
		}, 50);
	}

    eventShowSmileySelector = (event: Event): void => {
		let target = <HTMLElement>event.currentTarget;

		this.smileySelectorTargetTextArea = (<HTMLFormElement>target.closest('form')).querySelector('textarea');
		this.showSmileySelectorNearElement(target, this.eventInsertSmileyCode);
	}

	eventCloseSmileySelector = (): void => {
		let self = this;

		self.smileySelector.style.top = '0';
		self.smileySelector.style.left = '0';
		hide(self.smileySelector);

		self.body.removeEventListener('click', self.eventCloseSmileySelector);
		self.smileySelector.removeEventListener('click', self.eventStopPropagation);

		if (self.smileySelectorImageHandler) {
			self.smileySelector.querySelectorAll('img').forEach(element => {
				element.removeEventListener('click', self.smileySelectorImageHandler);
			});

			delete self.smileySelectorImageHandler;
		}
	}

	eventInsertSmileyCode = (event: Event): void => {
		let self = this;

		if (!self.smileySelectorTargetTextArea) {
			return;
		}

		let eventTarget = <Element>event.currentTarget
		let smileyCode = eventTarget.getAttribute('code') || '';
			   
		if (self.smileySelectorTargetTextArea.textContent !== '') {
			smileyCode = ` ${smileyCode} `;
		}

		insertAtCaret(self.smileySelectorTargetTextArea, smileyCode);

		self.eventCloseSmileySelector();
	}

	private eventStopPropagation = (event: Event) => {
		event.stopPropagation();
	}
}
