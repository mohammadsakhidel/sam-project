package com.ramanco.samandroid.objects;

import android.support.annotation.NonNull;

public class StickyHeader implements Comparable {

    String title;
    int startIndex;
    int bgColor;
    int textColor;

    public StickyHeader(String title, int startIndex, int bgColor, int textColor) {
        this.title = title;
        this.startIndex = startIndex;
        this.bgColor = bgColor;
        this.textColor = textColor;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public int getStartIndex() {
        return startIndex;
    }

    public void setStartIndex(int startIndex) {
        this.startIndex = startIndex;
    }

    public int getBgColor() {
        return bgColor;
    }

    public void setBgColor(int bgColor) {
        this.bgColor = bgColor;
    }

    public int getTextColor() {
        return textColor;
    }

    public void setTextColor(int textColor) {
        this.textColor = textColor;
    }

    @Override
    public int compareTo(@NonNull Object o) {
        if (o instanceof StickyHeader)
            return this.startIndex - ((StickyHeader) o).startIndex;
        else
            throw new ClassCastException("A ListViewSection object expected.");
    }
}
