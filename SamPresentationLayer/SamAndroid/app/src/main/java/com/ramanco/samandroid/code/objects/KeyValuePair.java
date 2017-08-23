package com.ramanco.samandroid.code.objects;

import android.support.annotation.NonNull;

public class KeyValuePair implements Comparable<KeyValuePair> {
    private String key;
    private String value;

    public KeyValuePair(String key, String value) {
        this.key = key;
        this.value = value;
    }

    public String getKey() {
        return key;
    }

    public void setKey(String key) {
        this.key = key;
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }

    @Override
    public int compareTo(@NonNull KeyValuePair o) {
        return this.getValue().compareTo(o.getValue());
    }
}
