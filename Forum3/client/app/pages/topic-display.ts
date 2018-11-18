﻿import { postToPath } from "../helpers";
import { Xhr } from "../services/xhr";
import { XhrOptions } from "../models/xhr-options";

// expects `document` to be defined at the global scope.
export default function () {
	let topicDisplay = new TopicDisplay(document);
	topicDisplay.setupPage();
}

export class TopicDisplay {
	constructor(private html: Document) { }

	setupPage() {
		this.html.querySelectorAll('.reply-button').forEach(element => {
			element.on('click', this.eventShowReplyForm)
		});

		this.html.querySelectorAll('.thought-button').forEach(element => {
			element.on('click', this.eventShowSmileySelector);
		});

		this.html.querySelectorAll('blockquote.reply').forEach(element => {
			element.on('click', this.eventShowFullReply);
		});

		this.html.querySelectorAll('[toggle-board]').forEach(element => {
			element.on('click', this.eventToggleBoard);
		});

		if ((<any>window).showFavicons) {
			this.html.querySelectorAll('.link-favicon').forEach(element => {
				element.show();
			});
		}
	}

	eventShowReplyForm = (event: Event) => {
		let target = <Element>event.currentTarget;

		this.html.querySelectorAll('.reply-form').forEach(element => {
			element.hide();
		});

		this.html.querySelectorAll('.reply-button').forEach(element => {
			element.off('click', this.eventShowReplyForm);
			element.on('click', this.eventShowReplyForm);
		});

		this.html.querySelectorAll('.reply-button').forEach(element => {
			element.off('click', this.eventHideReplyForm);
		});

		target.off('click', this.eventShowReplyForm);
		target.closest('section').querySelectorAll('.reply-form').forEach(element => { element.show(); });
		target.on('click', this.eventHideReplyForm);
	}

	eventHideReplyForm = (event: Event) => {
		let target = <Element>event.currentTarget;

		target.off('click', this.eventHideReplyForm);
		target.closest('section').querySelectorAll('.reply-form').forEach(element => { element.hide(); });
		target.on('click', this.eventShowReplyForm);
	}

	eventShowSmileySelector = (event: Event) => {
		event.preventDefault();
		let target = <Element>event.currentTarget;

		var messageId = target.getAttribute('message-id');

		ShowSmileySelector(event, function (smileyImg: Element) {
			var smileyId = smileyImg.getAttribute('smiley-id');

			postToPath('/Messages/AddThought', [
				{ 'MessageId': messageId },
				{ 'SmileyId': smileyId }
			]);

			this.eventCloseSmileySelector();
		});
	}

	eventShowFullReply = (event: Event) => {
		let target = <Element>event.currentTarget;

		target.off('click', this.eventCloseFullReply);
		target.on('click', this.eventCloseFullReply);

		target.querySelectorAll('.reply-preview').forEach(element => { element.hide() });
		target.querySelectorAll('.reply-body').forEach(element => { element.show() });
	}

	eventCloseFullReply = (event: Event) => {
		let target = <Element>event.currentTarget;

		target.querySelectorAll('.reply-body').forEach(element => { element.hide() });
		target.querySelectorAll('.reply-preview').forEach(element => { element.show() });

		target.off('click', this.eventShowFullReply);
		target.on('click', this.eventShowFullReply);
	}

	eventToggleBoard = (event: Event) => {
		event.stopPropagation();

		if ((<any>event.currentTarget).toggling)
			return;

		(<any>event.currentTarget).toggling = true;

		if ((<any>window).assignedBoards === undefined || (<any>window).togglePath === undefined)
			return;

		let assignedBoards = (<any>window).assignedBoards;

		let boardId = parseInt((<Element>event.currentTarget).getAttribute('board-id'));

		let imgSrc = this.html.querySelector(`[board-flag=${boardId}]`).getAttribute('src');

		if (assignedBoards.includes(boardId)) {
			assignedBoards.remove(boardId);
			imgSrc = imgSrc.replace('checked', 'unchecked');
		}
		else {
			assignedBoards.push(boardId);
			imgSrc = imgSrc.replace('unchecked', 'checked');
		}

		this.html.querySelector(`[board-flag=${boardId}]`).setAttribute('src', imgSrc);

		let request = Xhr.request(new XhrOptions({
			url: `${(<any>window).togglePath}&BoardId=${boardId}`			
		}));

		request.then(() => {
			(<any>event.currentTarget).toggling = false;
		});
	}
}